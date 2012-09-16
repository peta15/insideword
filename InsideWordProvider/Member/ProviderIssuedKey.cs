using System;
using System.Linq;
using System.Data.Objects;
using InsideWordResource;
using System.Collections.Generic;

namespace InsideWordProvider
{
    public class ProviderIssuedKey : ProviderAlternateMemberId
    {
        public ProviderIssuedKey() : base() { }
        public ProviderIssuedKey(long id) : base(id) { }

        public string IssuedKey
        {
            get { return _entityAlternateMemberId.AlternateId; }
        }

        public new bool IsNonce
        {
            get { return _entityAlternateMemberId.IsNonce; }
            set { _entityAlternateMemberId.IsNonce = value; }
        }

        public bool SetExpiryToMonth(int monthOffset)
        {
            DateTime endOfThisMonth = DateTime.UtcNow;
            // make sure that AddMonths is LAST in this order of operations otherwise we can get some very strange situations
            // i.e. February only has 28 days but January can have 31, so going from January 31st to February could yield some unexpected scenario
            _entityAlternateMemberId.ExpiryDate = endOfThisMonth.AddDays(1 - endOfThisMonth.Day)
                                                                .AddHours( - endOfThisMonth.Hour)
                                                                .AddMinutes( - endOfThisMonth.Minute)
                                                                .AddSeconds( - endOfThisMonth.Second)
                                                                .AddMonths(monthOffset);
            return true;
        }

        /// <summary>
        /// Function attempts to load a single issued key based on a specific criterion.
        /// </summary>
        /// <param name="memberId">member id of the issued key's owner</param>
        /// <param name="data">data stored with the issued key. Use null if the key has no data.</param>
        /// <param name="isNonce">nonce value for the key. true indicates we wish to load an issued key that is a nonce and false indicates otherwise.</param>
        /// <param name="monthOffset">the month offset from today's month of when the key will expire. If set to null then it will search for a key that doesn't expire.</param>
        /// <returns></returns>
        public bool LoadBy(long memberId, string data, bool isNonce, int? monthOffset)
        {
            int? month = null;
            if (monthOffset.HasValue)
            {
                month = (DateTime.UtcNow.Month + monthOffset.Value)%12;
                month = month == 0 ? 12 : month;
            }
            int intAltType = (int)AlternateType.IssuedKey;
            AlternateMemberId altId = DbCtx.Instance.AlternateMemberIds
                                                    .Where(anAltId => anAltId.MemberId == memberId &&
                                                                        anAltId.AlternateType == intAltType &&
                                                                        (data == null ? anAltId.Data == null : anAltId.Data == data) &&
                                                                        anAltId.IsNonce == isNonce &&
                                                                        (month == null ? anAltId.ExpiryDate == null : anAltId.ExpiryDate.Value.Month == month)
                                                        )
                                                    .SingleOrDefault();
            return Load(altId);
        }

        /// <summary>
        /// Method attemps to load an Issued Key with the following parameters. If it fails to load it will create the key instead.
        /// </summary>
        /// <param name="memberId">member Id of the issued key we wish to load/set, or set if we fail to load</param>
        /// <param name="data">data of the issued key we wish to load/set, or set if we fail to load</param>
        /// <param name="isNonce">nonce value of the issued key we wish to load/set, or set if we fail to load</param>
        /// <param name="monthOffset">month offset value of the issued key we wish to load, or set if we fail to load</param>
        /// <param name="isValidated">validation state we wish to set if we fail to load </param>
        /// <returns></returns>
        public bool LoadOrCreate(long memberId, string data, bool isNonce, int? monthOffset, bool isValidated)
        {
            bool returnValue = LoadBy(memberId, data, isNonce, monthOffset);
            if (!returnValue)
            {
                // key doesn't exist so create an issued key now to send back so that they can embed the key in their site.
                this.CreateDate = DateTime.UtcNow;
                this.EditDate = DateTime.UtcNow;
                if (monthOffset.HasValue)
                {
                    this.SetExpiryToMonth(monthOffset.Value);
                }
                this.IsNonce = isNonce;
                this.IsValidated = isValidated;
                this.Data = data;
                this.MemberId = memberId;
                this.Save();
            }
            return returnValue;
        }

         //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================    
        protected ProviderIssuedKey(AlternateMemberId anAltType) : base(anAltType) { }

        protected override void EntityClear()
        {
            base.EntityClear();
            _entityAlternateMemberId.AlternateType = (int)AlternateType.IssuedKey;
            _entityAlternateMemberId.AlternateId = _GenerateIssuedKey();
            _entityAlternateMemberId.IsValidated = true;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static protected UniqueKeyGenerator _keyGenerator;

        // Static constructor
        static ProviderIssuedKey()
        {
            _keyGenerator = new UniqueKeyGenerator();
        }

        static protected Converter<AlternateMemberId, ProviderIssuedKey> _converterEntityToProvider = new Converter<AlternateMemberId, ProviderIssuedKey>(_EntityToProvider);
        static protected ProviderIssuedKey _EntityToProvider(AlternateMemberId anAltType)
        {
            return new ProviderIssuedKey(anAltType);
        }

        static protected string _GenerateIssuedKey()
        {
            string returnValue = _keyGenerator.RandomAlphaNumeric();
            while (_Exists(returnValue, true, AlternateType.IssuedKey) != null)
            {
                returnValue = _keyGenerator.RandomAlphaNumeric();
            }
            return returnValue;
        }

        static public long? FindOwner(string issuedKey)
        {
            return _FindOwner(issuedKey, true, AlternateType.IssuedKey);
        }
    }
}