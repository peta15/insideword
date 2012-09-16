using System;
using System.Text;
using System.Collections.Generic;

namespace InsideWordResource
{
    public class UniqueKeyGenerator
    {
        protected const string _alphaNumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        protected const int _alphaNumericSize = 62;
        protected Random _rand;
        protected Object _counterLock;
        protected long _counter;
        protected Object _dateUniqueLock;
        protected long _prevTick;
        protected List<string> _usedKeyList;

        public UniqueKeyGenerator()
        {
            _rand = new Random((int)DateTime.UtcNow.Ticks);
            _counterLock = new Object();
            _counter = 0;
            _dateUniqueLock = new Object();
            _prevTick = DateTime.UtcNow.Ticks;
            _usedKeyList = new List<string>();
        }

        /// <summary>
        /// The current value of the counter. When the class is first created the counter always starts at 0.
        /// This function is thread safe.
        /// </summary>
        /// <returns></returns>
        public long Counter
        {
            get
            {
                lock (_counterLock)
                {
                    return _counter;
                }
            }
            set
            {
                lock (_counterLock)
                {
                    _counter = value;
                }
            }
        }

        /// <summary>
        /// Increments the current value of the counter.
        /// This function is thread safe.
        /// </summary>
        /// <returns></returns>
        public long IncrementCounter()
        {
            lock (_counterLock)
            {
                _counter++;
            }
            return _counter;
        }

        /// <summary>
        /// Function is just like RandomDigits, except it will never return the same random digits twice.
        /// This function is thread safe.
        /// </summary>
        /// <param name="size">number of random digits we wish to have</param>
        /// <param name="noRepeats">if set to true then the number will not match any previously generated number</param>
        /// <returns>long with number of random digits equal to size</returns>
        public long RandomDigits(int size, bool noRepeats)
        {
            if (size > 18)
            {
                throw new Exception("Can have no more than 18 digits in a long");
            }

            long returnValue = 0;
            bool duplicate = false;
            do
            {
                lock (_rand)
                {
                    returnValue = (_rand.Next() % 9) + 1;
                    returnValue *= 10;
                    for (int count = 1; count < size; count++)
                    {
                        returnValue = (_rand.Next() % 10);
                        returnValue *= 10;
                    }
                }

                if(noRepeats)
                {
                    lock (_usedKeyList)
                    {
                        duplicate = _usedKeyList.Exists(key => returnValue.CompareTo(key) == 0);
                    }
                }
            } while (duplicate && noRepeats);
            _usedKeyList.Add(returnValue.ToString());
            return returnValue;
        }

        /// <summary>
        /// Function returns a number where every digit is randomly chosen with equal probability.
        /// The only exception is the most significant digit being chosen from 1 to 9 instead of 0 to 9.
        /// The function will never return the same value twice.
        /// This function is thread safe.
        /// </summary>
        /// <param name="size">number of random digits we wish to have</param>
        /// <returns>long with number of random digits equal to size</returns>
        public long RandomDigits(int size)
        {
            return RandomDigits(size, true);
        }

        /// <summary>
        /// Function returns a number with 18 digits where every digit is randomly chosen with equal probability.
        /// The only exception is the most significant digit being chosen from 1 to 9 instead of 0 to 9.
        /// The function will never return the same value twice.
        /// This function is thread safe.
        /// </summary>
        /// <returns>long with 18 random digits</returns>
        public long RandomDigits()
        {
            return RandomDigits(18, true);
        }

        /// <summary>
        /// Function returns an alpha numeric string where every character is randomly chosen with equal probability.
        /// This function is thread safe.
        /// </summary>
        /// <param name="size">number of random characters in the return value</param>
        /// <param name="noRepeats">if set to true then the number will not match any previously generated number</param>
        /// <returns>string with the number of random characters equal to size</returns>
        public string RandomAlphaNumeric(int size, bool noRepeats)
        {
            string returnValue = "";
            bool duplicate = false;
            do
            {
                lock (_rand)
                {
                    StringBuilder sb = new StringBuilder("");
                    while (sb.Length < size)
                    {
                        sb.Append(_alphaNumeric[_rand.Next() % _alphaNumericSize]);
                    }
                    returnValue = sb.ToString();
                }

                if (noRepeats)
                {
                    lock (_usedKeyList)
                    {
                        duplicate = _usedKeyList.Exists(key => returnValue.CompareTo(key) == 0);
                    }
                }
            } while (duplicate && noRepeats);
            _usedKeyList.Add(returnValue.ToString());
            return returnValue;
        }

        /// <summary>
        /// Function returns an alpha numeric string where every character is randomly chosen with equal probability.
        /// The function will never return the same string twice.
        /// This function is thread safe.
        /// </summary>
        /// <param name="size">number of random characters in the return value</param>
        /// <returns>string with the number of random characters equal to size</returns>
        public string RandomAlphaNumeric(int size)
        {
            return RandomAlphaNumeric(size, true);
        }

        /// <summary>
        /// Function returns a 16 character alpha numeric string where every character is randomly chosen with equal probability.
        /// The function will never return the same string twice.
        /// This function is thread safe.
        /// </summary>
        /// <returns>string with 16 random characters</returns>
        public string RandomAlphaNumeric()
        {
            return RandomAlphaNumeric(16, true);
        }

        /// <summary>
        /// Function returns a set of alphanumeric characters that are generated based on the current time and are guaranteed to be always unique.
        /// </summary>
        /// <returns>string of alphanumeric characters</returns>
        public string TimeUniqueAlphaNumeric()
        {
            StringBuilder returnValue = new StringBuilder();
            long tick;

            // lock this section to give some extra help in ensuring a unique tick
            lock (_dateUniqueLock)
            {
                tick = DateTime.UtcNow.Ticks;
                while (tick == _prevTick)
                {
                    tick = DateTime.UtcNow.Ticks;
                }
                _prevTick = tick;
            }

            while(tick > 0)
            {
                returnValue.Append( _alphaNumeric[ (int)(tick % _alphaNumericSize) ] );
                tick /= _alphaNumericSize;
            }
            return returnValue.ToString();
        }
    }
}
