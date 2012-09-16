using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace InsideWordResource
{
    static public class IWStringUtility
    {
        static public bool TryParse(string input, out MailAddress output)
        {
            output = null;
            try { output = new MailAddress(input); }
            catch { }
            return output != null;
        }

        static public bool TryUriConcat(Uri address, string subFolder, out Uri output)
        {
            string path = address.AbsoluteUri;
            if (!string.IsNullOrEmpty(subFolder))
            {
                if (path.EndsWith("/"))
                {
                    if (subFolder.StartsWith("/"))
                    {
                        path += subFolder.Substring(1);
                    }
                    else
                    {
                        path += subFolder;
                    }
                }
                else
                {
                    if (subFolder.StartsWith("/"))
                    {
                        path += subFolder;
                    }
                    else
                    {
                        path += "/"+subFolder;
                    }
                }
            }
            return Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out output);
        }

        static public bool TryUrlDecode(string input, out string output, string defaultValue = null)
        {
            output = defaultValue;
            if (!string.IsNullOrWhiteSpace(input))
            {
                try { output = Uri.UnescapeDataString(input); }
                catch { }
            }

            return output != null;
        }

        static public bool Try64bitDecode(string input, out string output)
        {
            output = null;
            try
            {
                byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(input);
                output = Convert.ToBase64String(encbuff);
            }
            catch {}

            return output != null;
        }

        static public string TruncateClean(string input, int maxLength)
        {
            if (input.Length > maxLength)
            {
                input = input.Substring(0, maxLength - 3) + "...";
            }

            return input;
        }

        static public string SuffixedNumber(long number)
        {
            string suffix = "th";
		    switch(number%10)
		    {
			    case 1:
				    if(number != 11)
				    {
                        suffix = "st";
				    }
			    break;
			    case 2:
				    if(number != 12)
				    {
                        suffix = "nd";
				    }
			    break;
			    case 3:
				    if(number != 13)
				    {
                        suffix = "rd";
				    }
			    break;
		    }
            return number+suffix;
        }
    }
}
