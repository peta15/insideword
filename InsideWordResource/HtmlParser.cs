using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Collections;
using System.Text;
using HtmlAgilityPack;

namespace InsideWordResource
{
    /// <summary>
    /// Html parsing for submitted articles via web interface, email etc.  Blacklist malicious and invalid html tags, parse for blurb images and more.
    /// </summary>
    public class HtmlParser
    {
        public HtmlParser()
        {
            AllowedTagList = new List<string> { "p", "strong", "em", "u", "h1", "h2", "h3", "h4", "h5", "h6", "img", "li", 
                                                "ol", "ul", "span", "div", "a", "blockquote", "object", "param", "embed", 
                                                "br", "i", "del", "cite", "table", "tr", "td", "th", "tbody", "thead", "tfoot",
                                                "address", "pre"
            };

            AllowedAttributeList = new List<string> { "id", "class", "href", "style", "width", "height", "name", "value", "src", "alt",  
                                                      "type", "allowscriptaccess", "allowfullscreen", "data", "title", "bgcolor"
            };
        }

        public HtmlParser(string input) : this()
        {
            HtmlString = input;
        }

        public string HtmlString
        {
            get
            {
                return HtmlDoc.DocumentNode.WriteContentTo();
            }
            set
            {
                HtmlDoc = new HtmlDocument();
                HtmlDoc.OptionWriteEmptyNodes = true;
                HtmlDoc.LoadHtml(value);
            }
        }

        public HtmlDocument HtmlDoc { get; set; }
        public List<string> AllowedTagList { get; set; }
        public List<string> AllowedAttributeList { get; set; }

        /// <summary>
        /// Parse 'dirty' Html submitted by the user with the HtmlAgilityPack.  Whitelist allowed html tags and attributes (for tinymce) and deny all others.
        /// Perform any other html parsing needed.
        /// Optionally return all inner text by stripping out all tags but keeping inner text of valid nodes
        /// </summary>
        /// <param name="input"></param>
        /// <param name="allowedTags"></param>
        /// <param name="allowedAttributes"></param>
        /// <param name="innerTextOnly"></param>
        /// <returns></returns>
        public string StripTagsAndAttributes(List<string> allowedTags, List<string> allowedAttributes, bool innerTextOnly)
        {
            string returnValue = null;

            if (allowedTags == null)
            {
                allowedTags = this.AllowedTagList;
            }

            if (allowedAttributes == null)
            {
                allowedAttributes = this.AllowedAttributeList;
            }

            HtmlDocument doc = HtmlDoc;

            // whitelist tags and attributes

            HtmlNodeCollection allNodes = doc.DocumentNode.SelectNodes("//*"); // xpath to select all nodes in document
            if (allNodes != null) // will just return the text represendation of the document if no nodes
            {
                foreach (HtmlNode node in allNodes)
                {
                    if (!allowedTags.Contains(node.Name))
                    {
                        // remove denied tags
                        node.Remove();
                    }
                    else
                    {
                        // remove denied attributes
                        ParseAttributes(allowedAttributes, node);
                    }
                }

                if (innerTextOnly)
                {
                    returnValue = _regexHtmlTag.Replace(doc.DocumentNode.WriteContentTo(), "");
                }
            }

            if(returnValue == null)
            {
                returnValue = doc.DocumentNode.WriteContentTo();
            }

            return returnValue;
        }

        /// <summary>
        /// Method removes all html from the document. This is a one way transformation.
        /// </summary>
        /// <returns>a string with no html tags present at all</returns>
        public string HtmlFree()
        {
            return this.StripTagsAndAttributes(null, null, true);
        }


        /// <summary>
        /// Method removes all potentially dangerous or malicious html tags and attributes from the string.
        /// </summary>
        /// <returns>a string with no unsafe html tags and attributes.</returns>
        public string InjectionHtmlFree()
        {
            return this.StripTagsAndAttributes(null, null, false);
        }

        /// <summary>
        /// Get absolute image infos from document
        /// </summary>
        /// <param name="validUriHost">restricts images to the local domain or the domains specified (www.insideword.com, www.chunkng.com, ...)</param>
        /// <returns>List of image infos whose uri was valid and absolute</returns>
        public List<ImageInfo> GetImageInfos(List<string> validUriHosts)
        {
            HtmlNodeCollection imageNodeList = HtmlDoc.DocumentNode.SelectNodes("//img");
            List<ImageInfo> imageInfoList = new List<ImageInfo>();
            ImageInfo anImageInfo;
            if (imageNodeList != null)
            {
                foreach (HtmlNode imageNode in imageNodeList)
                {
                    string srcAttr = imageNode.GetAttributeValue("src", String.Empty);
                    if (srcAttr != String.Empty)
                    {
                        Uri hrefUri;
                        if (Uri.TryCreate(srcAttr, UriKind.RelativeOrAbsolute, out hrefUri))
                        {
                            if (hrefUri.IsAbsoluteUri &&
                                (validUriHosts == null || validUriHosts.Count == 0 || validUriHosts.Contains(hrefUri.Host)))
                            {
                                anImageInfo = new ImageInfo();
                                anImageInfo.Src = hrefUri.AbsoluteUri;
                                anImageInfo.Alt = imageNode.GetAttributeValue("alt", string.Empty);
                                anImageInfo.Height = (short)imageNode.GetAttributeValue("height", 0);
                                anImageInfo.Width = (short)imageNode.GetAttributeValue("width", 0);
                                imageInfoList.Add(anImageInfo);
                            }
                            else
                            {
                                // Relative Uris not supported right now!
                                // TODO try to resolve the relative uris to absolute on any of our valid domains
                                // (check the actual paths for content or ignore all non-absolute urls
                                // imageUris.Add(uri.AbsolutePath);
                            }
                        }
                    }
                }
            }
            return imageInfoList;
        }

        //=========================================================
        // STATIC FUNCTIONS
        //=========================================================
        static protected Regex _regexHtmlTag = new Regex(@"(\<[^\>]*\>)");
        static protected Regex _regexHtmlSpecialChars = new Regex(@"\&\w+\;");

        /// <summary>
        /// Function removes all html from the given input.
        /// NOTE Use only if it is the only function you will call on the document, otherwise use the non-static versions
        /// </summary>
        /// <param name="input">input we wish to strip the html from</param>
        /// <returns>a string with no html tags present at all</returns>
        static public string HtmlFree(string input)
        {
            return new HtmlParser(input).StripTagsAndAttributes(null, null, true);
        }


        /// <summary>
        /// Function removes all potentially dangerous or malicious html tags and attributes from the given input.
        /// NOTE Use only if it is the only function you will call on the document, otherwise use the non-static versions
        /// </summary>
        /// <param name="input">input we wish to strip the unsafe html tags and attributes from.</param>
        /// <returns>a string with no unsafe html tags and attributes.</returns>
        static public string InjectionHtmlFree(string input)
        {
            return new HtmlParser(input).StripTagsAndAttributes(null, null, false);
        }

        /// <summary>
        /// Function removes all potentially dangerous or malicious html tags and attributes from the given input.
        /// NOTE Use only if it is the only function you will call on the document, otherwise use the non-static versions
        /// </summary>
        /// <param name="input">input we wish to strip the unsafe html tags and attributes from.</param>
        /// <returns>a string with no unsafe html tags and attributes.</returns>
        static public string StripSpecialChars(string input)
        {
            return _regexHtmlSpecialChars.Replace(input, " ");
        }

        /// <summary>
        /// Get absolute image infos from html string
        /// </summary>
        /// <param name="input">input we wish to extract the images from.</param>
        /// <param name="validUriHost">restricts images to the local domain or the domains specified (www.insideword.com, www.chunkng.com, ...)</param>
        /// <returns>List of image infos whose uri was valid and absolute</returns>
        protected const string _sRegexImg = @"<img (href=[^<>]*|alt=[^<>]*|width=[^<>]*|height=[^<>]*)*(/>|>.*</img>)";
        static public List<ImageInfo> GetImageInfos(string htmlString, List<string> validUriHosts)
        {
            return new HtmlParser(htmlString).GetImageInfos(validUriHosts);
        }

        protected const string _sRegexUri = @"(?:https?://(?:www.)?[^<>\s]+)";
        static protected Regex _regexUnenclosedUri = new Regex(@"(<a(?:\s[^<>]*)?>(?:.|\s)*?</a>)|(=\s*('|\"")\s*" + _sRegexUri + @"\s*\3\s*)|(" + _sRegexUri + ")");
        static public string EncloseUriLink(string htmlText)
        {
            return _regexUnenclosedUri.Replace(htmlText,
                                               match => (match.Groups[1].Success)?
                                                            match.Groups[1].Value:
                                                            ((match.Groups[2].Success)?
                                                                match.Groups[2].Value:
                                                                "<a href='"+match.Groups[4].Value+"'>"+match.Groups[4].Value+"</a>"));
        }

        //=========================================================
        // PRIVATE FUNCTIONS
        //=========================================================

        private static void ParseAttributes(List<string> allowedAttributes, HtmlNode node)
        {
            List<HtmlAttribute> attributesToRemoveFromNode = new List<HtmlAttribute>();
            string attrName = null;

            foreach (HtmlAttribute attribute in node.Attributes)
            {
                attrName = attribute.Name.ToLower();
                if (!allowedAttributes.Contains(attrName))
                {
                    attributesToRemoveFromNode.Add(attribute);
                }
                else
                {
                    // blacklist harmful script execution http://htmlagilitypack.codeplex.com/Thread/View.aspx?ThreadId=24346, 
                    // http://www.codingforums.com/archive/index.php/t-47150.html,
                    // http://cf-bill.blogspot.com/2008/07/c-parsing-url-for-its-component-parts.html

                    //strip links if the link starts with anything other than http (such as jscript, javascript, vbscript, mailto, ftp, etc.) if it starts with the protocol
                    if (attrName == "src" || attrName == "href")
                    {
                        Uri uri = null;
                        if (!string.IsNullOrWhiteSpace(attribute.Value) &&
                            Uri.TryCreate(attribute.Value, UriKind.RelativeOrAbsolute, out uri))
                        {
                            if (uri.IsAbsoluteUri)
                            {
                                if (uri.Scheme != null && uri.Scheme != "http" && uri.Scheme != "https")
                                {
                                    attribute.Value = "#";  // set the uri to go nowhere
                                }
                            }
                            else
                            {
                                // this is a relative url that doesn't have a scheme so ignore
                            }
                        }
                    }
                    else if (attrName == "style" && attribute.Value.ToLower().Contains("expression"))
                    {
                        //strip any "style" attributes that contain the word "expression" (IE evaluates this as a script)
                        attribute.Value = string.Empty;
                    }
                }
            }

            // perform the removal outside of the foreach otherwise it will mess with it
            foreach (HtmlAttribute attribute in attributesToRemoveFromNode)
            {
                node.Attributes.Remove(attribute.Name);
            }
        }
    }
}