using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Moq;
using InsideWordResource;

namespace InsideWordResourceTests
{
    public class InjectionHtmlFree
    {
        [Fact]
        public void Should_allow_style_attribute()
        {
            string input = "<p>nostye<span style=\"font-size: large;\">styled</span></p>";
            string output = HtmlParser.InjectionHtmlFree(input);
            Assert.Equal(input, output);
        }

        [Fact]
        public void Should_deny_script_tags_and_their_contents()
        {
            string input = "<script>alert();</script>";
            string output = HtmlParser.InjectionHtmlFree(input);
            Assert.Empty(output);
        }

        [Fact]
        public void Should_deny_style_tags_and_their_contents()
        {
            string input = "<style>.div { background: red; }</style>";
            string output = HtmlParser.InjectionHtmlFree(input);
            Assert.Empty(output);
        }

        [Fact]
        public void Should_strip_first_attribute_but_not_second()
        {
            string input = "<span invalidAttribute=\"begone\" style=\"valid\">test</span>";
            string output = HtmlParser.InjectionHtmlFree(input);
            string expected = "<span style=\"valid\">test</span>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_allow_youTube_embed()
        {
            string input = "<object width=\"640\" height=\"385\"><param name=\"movie\" value=\"http://www.youtube.com/v/A6LYLJbDQxc?fs=1&amp;hl=en_US\">"
                           + "</param><param name=\"allowFullScreen\" value=\"true\"></param><param name=\"allowscriptaccess\" value=\"always\"></param>"
                           + "<embed src=\"http://www.youtube.com/v/A6LYLJbDQxc?fs=1&amp;hl=en_US\" type=\"application/x-shockwave-flash\" "
                           + "allowscriptaccess=\"always\" allowfullscreen=\"true\" width=\"640\" height=\"385\"></embed></object>";
            string output = HtmlParser.InjectionHtmlFree(input);
            string expected = "<object width=\"640\" height=\"385\"><param name=\"movie\" value=\"http://www.youtube.com/v/A6LYLJbDQxc?fs=1&amp;hl=en_US\" />"
                           + "<param name=\"allowFullScreen\" value=\"true\" /><param name=\"allowscriptaccess\" value=\"always\" />"
                           + "<embed src=\"http://www.youtube.com/v/A6LYLJbDQxc?fs=1&amp;hl=en_US\" type=\"application/x-shockwave-flash\" "
                           + "allowscriptaccess=\"always\" allowfullscreen=\"true\" width=\"640\" height=\"385\" /></object>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_allow_youTube_embed_all_tinyMce_options()
        {
            string input = "<p>"
                + "<object id=\"myFlashMovieId\" style=\"width: 640px; height: 385px;\" width=\"640\" height=\"385\" data=\"http://www.youtube.com/v/A6LYLJbDQxc?fs=1&amp;hl=en_US\" type=\"application/x-shockwave-flash\">"
                    + "<param name=\"quality\" value=\"high\" />"
                    + "<param name=\"scale\" value=\"showall\" />"
                    + "<param name=\"salign\" value=\"t\" />"
                    + "<param name=\"wmode\" value=\"opaque\" />"
                    + "<param name=\"base\" value=\"abc\" />"
                    + "<param name=\"flashvars\" value=\"abc\" />"
                    + "<param name=\"name\" value=\"myFlashMovieName\" />"
                    + "<param name=\"src\" value=\"http://www.youtube.com/v/A6LYLJbDQxc?fs=1&amp;hl=en_US\" />"
                    + "<param name=\"bgcolor\" value=\"#3af20c\" />"
                    + "<param name=\"vspace\" value=\"2\" />"
                    + "<param name=\"hspace\" value=\"3\" />"
                + "</object>"
                + "</p>";
            string output = HtmlParser.InjectionHtmlFree(input);
            Assert.Equal(input, output);
        }

        [Fact]
        public void Should_strip_dangerous_anchor_tags_with_javascript()
        {
            string input = "<p>innocuous text <A HREF=\"javascript:alert('hack')\">Dangerous link</A> more innocuous text</p>";
            string output = HtmlParser.InjectionHtmlFree(input);
            string expected = "<p>innocuous text <a href=\"#\">Dangerous link</a> more innocuous text</p>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_remove_disallowed_tag()
        {
            string input = "<iamnotanhtmltag/><iamnotanhtmltag2><iamnotanhtmltag2/>";
            string output = HtmlParser.InjectionHtmlFree(input);
            Assert.Empty(output);
        }

        [Fact]
        public void Should_allow_br_tag()
        {
            string input = "bob<br>likes<br />endlines</br/>very<br/>much.  crazy br tag</br/asdfasdfaijgioaejrhgarg>";
            string output = HtmlParser.InjectionHtmlFree(input);
            string expected = "bob<br />likes<br>endlines</br>very<br>much.  crazy br tag</br>"; // html agility pack matches open and closing tags even if the html is invalid xhtml
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_wipe_valid_tags_inside_invalid_tag()
        {
            string input = "text<invalidcontainer>more text<p>paragraph</p></invalidcontainer>";
            string output = HtmlParser.InjectionHtmlFree(input);
            string expected = "text";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_allow_https_anchor()
        {
            string input = "<a href='https://vault.fbi.gov/hottel_guy/Guy%20Hottel%20Part%201%20of%201/view?searchterm=Guy%20Hottel'>blah</a>";
            string output = HtmlParser.InjectionHtmlFree(input);
            string expected = "<a href='https://vault.fbi.gov/hottel_guy/Guy%20Hottel%20Part%201%20of%201/view?searchterm=Guy%20Hottel'>blah</a>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_allow_img()
        {
            string input = "<img src='http://theunitedpersons.org/wordpress/wp-content/uploads/2011/04/ufo.jpg' alt='' title='ufo' width='564' height='424' class='aligncenter size-full wp-image-7392' />";
            string output = HtmlParser.InjectionHtmlFree(input);
            string expected = "<img src='http://theunitedpersons.org/wordpress/wp-content/uploads/2011/04/ufo.jpg' alt='' title='ufo' width='564' height='424' class='aligncenter size-full wp-image-7392' />";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_allow_cite()
        {
            string input = "<cite><a href='https://www.economist.com/research/articlesBySubject/displaystory.cfm?subjectid=14391731&story_id=18437755'>Schumpeter</a></cite>";
            string output = HtmlParser.InjectionHtmlFree(input);
            string expected = "<cite><a href='https://www.economist.com/research/articlesBySubject/displaystory.cfm?subjectid=14391731&story_id=18437755'>Schumpeter</a></cite>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_allow_table()
        {
            string input = "<div>January    </div>  <table width='100%'>  <tbody>  <tr bgcolor='#7FAAFF'>  <th>Release Date</th>  <th> Film-Title</th>  <th> Star-Cast</th>  <th> Director</th>  <th>Genre</th>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 7</td>  <td><a href='http://www.top10bollywood.com/2010/12/ashoka-hero-movie-information.html'>Ashoka The Hero</a></td>  <td>-</td>  <td>Gaurav Jain</td>  <td>Animation</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 7</td>  <td><a href='http://www.top10bollywood.com/2010/10/impatient-vivek.html'>Impatient Vivek</a></td>  <td>Vivek Sudarshan, Sayali Bhagat</td>  <td>Rahat Kazmi</td>  <td>Comedy, Romance</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 7</td>  <td><a href='http://www.top10bollywood.com/2010/10/no-one-killed-jessica.html'>No One Killed Jessica</a></td>  <td>Vidya Balan, Rani Mukherjee</td>  <td>Rajkumar Gupta</td>  <td>Social, Thriller</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 7</td>  <td><a href='http://www.top10bollywood.com/2010/12/vikalp-movie-information.html'>Vikalp</a></td>  <td>Deepal Shaw, Alok Nath, Chetan Pandit</td>  <td>Sachin P. Karande</td>  <td>Thriller</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 14</td>  <td><a href='http://www.top10bollywood.com/2010/12/mumbai-mast-kallander-movie-information.html'>Mumbai Mast Kallander</a></td>  <td>Shilpa Shukla, Rajesh Vivek, Rajendra Sethi</td>  <td>Aman Mihani</td>  <td>Comedy</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 14</td>  <td><a href='http://www.top10bollywood.com/2010/11/turning-30.html'>Turning 30!!!</a></td>  <td>Purab Kohli, Gul Panag, Siddharth Makkar, Tillotama Shome</td>  <td>Alankrita Shrivastava</td>  <td>Comedy</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 14</td>  <td><a href='http://www.top10bollywood.com/2010/11/yamla-pagla-deewana.html'>Yamla Pagla Deewana</a></td>  <td>Sunny Deol, Dharmendra, Bobby Deol, Kulraj Randhawa, Paresh Rawal</td>  <td>Sameer Karnik</td>  <td>Comedy</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 21</td>  <td><a href='http://www.top10bollywood.com/2010/11/dhobi-ghat.html'>Dhobi Ghat</a></td>  <td>Prateik Babbar, Aamir Khan, Monica Dogra, Kriti Malhotra</td>  <td>Kiran Rao</td>  <td>Social</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 21</td>  <td><a href='http://www.top10bollywood.com/2010/12/hostel-movie-information.html'>Hostel</a></td>  <td>Vatsal Sheth, Tulip Joshi, Mukesh Tiwari</td>  <td>Manish Gupta</td>  <td>Social</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 28</td>  <td><a href='http://www.top10bollywood.com/2010/11/dil-toh-bachcha-hai-ji.html'>Dil Toh Baccha Hai Ji</a></td>  <td>Ajay Devgn, Emraan Hashmi, Omi Vaidya, Amisha Patel, Tisca Chopra, Shruti Haasan</td>  <td>Madhur Bhandarkar</td>  <td>Romance, Comedy</td>  </tr>  </tbody>  </table>";
            string output = HtmlParser.InjectionHtmlFree(input);
            string expected = "<div>January    </div>  <table width='100%'>  <tbody>  <tr bgcolor='#7FAAFF'>  <th>Release Date</th>  <th> Film-Title</th>  <th> Star-Cast</th>  <th> Director</th>  <th>Genre</th>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 7</td>  <td><a href='http://www.top10bollywood.com/2010/12/ashoka-hero-movie-information.html'>Ashoka The Hero</a></td>  <td>-</td>  <td>Gaurav Jain</td>  <td>Animation</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 7</td>  <td><a href='http://www.top10bollywood.com/2010/10/impatient-vivek.html'>Impatient Vivek</a></td>  <td>Vivek Sudarshan, Sayali Bhagat</td>  <td>Rahat Kazmi</td>  <td>Comedy, Romance</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 7</td>  <td><a href='http://www.top10bollywood.com/2010/10/no-one-killed-jessica.html'>No One Killed Jessica</a></td>  <td>Vidya Balan, Rani Mukherjee</td>  <td>Rajkumar Gupta</td>  <td>Social, Thriller</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 7</td>  <td><a href='http://www.top10bollywood.com/2010/12/vikalp-movie-information.html'>Vikalp</a></td>  <td>Deepal Shaw, Alok Nath, Chetan Pandit</td>  <td>Sachin P. Karande</td>  <td>Thriller</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 14</td>  <td><a href='http://www.top10bollywood.com/2010/12/mumbai-mast-kallander-movie-information.html'>Mumbai Mast Kallander</a></td>  <td>Shilpa Shukla, Rajesh Vivek, Rajendra Sethi</td>  <td>Aman Mihani</td>  <td>Comedy</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 14</td>  <td><a href='http://www.top10bollywood.com/2010/11/turning-30.html'>Turning 30!!!</a></td>  <td>Purab Kohli, Gul Panag, Siddharth Makkar, Tillotama Shome</td>  <td>Alankrita Shrivastava</td>  <td>Comedy</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 14</td>  <td><a href='http://www.top10bollywood.com/2010/11/yamla-pagla-deewana.html'>Yamla Pagla Deewana</a></td>  <td>Sunny Deol, Dharmendra, Bobby Deol, Kulraj Randhawa, Paresh Rawal</td>  <td>Sameer Karnik</td>  <td>Comedy</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 21</td>  <td><a href='http://www.top10bollywood.com/2010/11/dhobi-ghat.html'>Dhobi Ghat</a></td>  <td>Prateik Babbar, Aamir Khan, Monica Dogra, Kriti Malhotra</td>  <td>Kiran Rao</td>  <td>Social</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 21</td>  <td><a href='http://www.top10bollywood.com/2010/12/hostel-movie-information.html'>Hostel</a></td>  <td>Vatsal Sheth, Tulip Joshi, Mukesh Tiwari</td>  <td>Manish Gupta</td>  <td>Social</td>  </tr>  <tr bgcolor='#f6f6f6'>  <td>Jan 28</td>  <td><a href='http://www.top10bollywood.com/2010/11/dil-toh-bachcha-hai-ji.html'>Dil Toh Baccha Hai Ji</a></td>  <td>Ajay Devgn, Emraan Hashmi, Omi Vaidya, Amisha Patel, Tisca Chopra, Shruti Haasan</td>  <td>Madhur Bhandarkar</td>  <td>Romance, Comedy</td>  </tr>  </tbody>  </table>";
            Assert.Equal(expected, output);
        }
    }

    public class EncloseUriTags
    {
        [Fact]
        public void Should_enclose_simple_url()
        {
            string input = "http://www.insideword.com";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<a href='http://www.insideword.com'>http://www.insideword.com</a>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_enclose_complex_url()
        {
            string input = "https://vault.fbi.gov/hottel_guy/Guy%20Hottel%20Part%201%20of%201/view?searchterm=Guy%20Hottel";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<a href='https://vault.fbi.gov/hottel_guy/Guy%20Hottel%20Part%201%20of%201/view?searchterm=Guy%20Hottel'>https://vault.fbi.gov/hottel_guy/Guy%20Hottel%20Part%201%20of%201/view?searchterm=Guy%20Hottel</a>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_anchor()
        {
            string input = "<a href='https://vault.fbi.gov/hottel_guy/Guy%20Hottel%20Part%201%20of%201/view?searchterm=Guy%20Hottel'>blah</a>";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<a href='https://vault.fbi.gov/hottel_guy/Guy%20Hottel%20Part%201%20of%201/view?searchterm=Guy%20Hottel'>blah</a>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_img()
        {
            string input = "<img src='http://theunitedpersons.org/wordpress/wp-content/uploads/2011/04/ufo.jpg' alt='' title='ufo' width='564' height='424' class='aligncenter size-full wp-image-7392' />";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<img src='http://theunitedpersons.org/wordpress/wp-content/uploads/2011/04/ufo.jpg' alt='' title='ufo' width='564' height='424' class='aligncenter size-full wp-image-7392' />";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_object()
        {
            string input = "<object width='640' height='505'><param name='movie' value='http://www.youtube-nocookie.com/v/pdxucpPq6Lc&#038;hl=en_US&#038;fs=1&#038;rel=0&#038;color1=0x3a3a3a&#038;color2=0x999999'></param><param name='allowFullScreen' value='true'></param><param name='allowscriptaccess' value='always'></param><embed src='http://www.youtube-nocookie.com/v/pdxucpPq6Lc&#038;hl=en_US&#038;fs=1&#038;rel=0&#038;color1=0x3a3a3a&#038;color2=0x999999' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='640' height='505'></embed></object>";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<object width='640' height='505'><param name='movie' value='http://www.youtube-nocookie.com/v/pdxucpPq6Lc&#038;hl=en_US&#038;fs=1&#038;rel=0&#038;color1=0x3a3a3a&#038;color2=0x999999'></param><param name='allowFullScreen' value='true'></param><param name='allowscriptaccess' value='always'></param><embed src='http://www.youtube-nocookie.com/v/pdxucpPq6Lc&#038;hl=en_US&#038;fs=1&#038;rel=0&#038;color1=0x3a3a3a&#038;color2=0x999999' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='640' height='505'></embed></object>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_endlines()
        {
            string input = "<object width='450' height='321' base='.' codebase='http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000'>                    <param name='movie' value='http://assets4.playedonline.com/system/files/dn8.swf'/>                    <param name='base' value='.'/>                    <param name='quality' value='high'/>                    <param value='always' name='AllowScriptAccess'/>                    <param value='exactfit' name='scale'/>                    <embed width='450' height='321' src='http://assets4.playedonline.com/system/files/dn8.swf' type='application/x-shockwave-flash' pluginspage='http://www.macromedia.com/go/getflashplayer' quality='high' allowscriptaccess='always'/>                  </object>";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<object width='450' height='321' base='.' codebase='http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000'>                    <param name='movie' value='http://assets4.playedonline.com/system/files/dn8.swf'/>                    <param name='base' value='.'/>                    <param name='quality' value='high'/>                    <param value='always' name='AllowScriptAccess'/>                    <param value='exactfit' name='scale'/>                    <embed width='450' height='321' src='http://assets4.playedonline.com/system/files/dn8.swf' type='application/x-shockwave-flash' pluginspage='http://www.macromedia.com/go/getflashplayer' quality='high' allowscriptaccess='always'/>                  </object>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_links()
        {
            string input = "<link rel='stylesheet' type='text/css' href='http://playedonline.com/javascripts/shadowbox/shadowbox.css'>";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<link rel='stylesheet' type='text/css' href='http://playedonline.com/javascripts/shadowbox/shadowbox.css'>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_scripts()
        {
            string input = "<script type='text/javascript' src='http://playedonline.com/javascripts/shadowbox/shadowbox.js'></script>";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<script type='text/javascript' src='http://playedonline.com/javascripts/shadowbox/shadowbox.js'></script>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_scripts_links_objects_anchors()
        {
            string input = "<link rel='stylesheet' type='text/css' href='http://playedonline.com/javascripts/shadowbox/shadowbox.css'><script type='text/javascript' src='http://playedonline.com/javascripts/shadowbox/shadowbox.js'></script><script type='text/javascript'>Shadowbox.init({modal:true});</script><a href='http://assets4.playedonline.com/system/files/dn8.swf' rel='shadowbox;height=400;width=560'><img src='http://playedonline.com/system/thumbnails/599770/regular/dn8.jpg?1304320946' style='float:left; margin:10px;'></a><div style='padding-top:5px;'><a href='http://assets4.playedonline.com/system/files/dn8.swf' rel='shadowbox;height=400;width=560'>Play</a><br/>This bullet hell shooter features randomly generated enemies, music and an upgrade system making no two games alike!<br/><!--more Read more--></div><div id='GameWrapper' style='clear:both;width:450px; height:330px; position:relative; margin:10px auto;'>            <script type='text/javascript'>function hidePreRoll() {clearTimeout(prerolltimer);document.getElementById('GameEmbed').style.visibility = 'visible';document.getElementById('PreRollAd').style.visibility = 'hidden';} prerolltimer = window.setTimeout(hidePreRoll, 15000);</script>              <div id='PreRollAd' style='position:absolute;top:0px;left:0px;right:0px;bottom:0px;'>              <div class='AdContent' style='text-align:center;fond-weight:bold; width:300px;margin:25px auto;'>  <p class='wordpress'>The game is loading<a href='http://affiliate.linkwise.gr/z/140771/CD622/'><img src='http://affiliate.linkwise.gr/42/622/140771/' alt='' border='0'></a>Advertisement - click to <a href='#' onclick='hidePreRoll(); return false;'>skip</a> </p>              </div>            </div>               <div id='GameEmbed' style='position:absolute;top:0px;left:0px;right:0px;bottom:0px;visibility:hidden;'>                  <object width='450' height='321' base='.' codebase='http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000'>                    <param name='movie' value='http://assets4.playedonline.com/system/files/dn8.swf'/>                    <param name='base' value='.'/>                    <param name='quality' value='high'/>                    <param value='always' name='AllowScriptAccess'/>                    <param value='exactfit' name='scale'/>                    <embed width='450' height='321' src='http://assets4.playedonline.com/system/files/dn8.swf' type='application/x-shockwave-flash' pluginspage='http://www.macromedia.com/go/getflashplayer' quality='high' allowscriptaccess='always'/>                  </object>              </div></div><br/><br/><div style='text-align:center; font-size:smaller;'><a href='http://www.playedonline.com/game/599770/dn8.html' style='text-decoration:none;'>DN8</a> is Powered by dailygame.org</div>";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<link rel='stylesheet' type='text/css' href='http://playedonline.com/javascripts/shadowbox/shadowbox.css'><script type='text/javascript' src='http://playedonline.com/javascripts/shadowbox/shadowbox.js'></script><script type='text/javascript'>Shadowbox.init({modal:true});</script><a href='http://assets4.playedonline.com/system/files/dn8.swf' rel='shadowbox;height=400;width=560'><img src='http://playedonline.com/system/thumbnails/599770/regular/dn8.jpg?1304320946' style='float:left; margin:10px;'></a><div style='padding-top:5px;'><a href='http://assets4.playedonline.com/system/files/dn8.swf' rel='shadowbox;height=400;width=560'>Play</a><br/>This bullet hell shooter features randomly generated enemies, music and an upgrade system making no two games alike!<br/><!--more Read more--></div><div id='GameWrapper' style='clear:both;width:450px; height:330px; position:relative; margin:10px auto;'>            <script type='text/javascript'>function hidePreRoll() {clearTimeout(prerolltimer);document.getElementById('GameEmbed').style.visibility = 'visible';document.getElementById('PreRollAd').style.visibility = 'hidden';} prerolltimer = window.setTimeout(hidePreRoll, 15000);</script>              <div id='PreRollAd' style='position:absolute;top:0px;left:0px;right:0px;bottom:0px;'>              <div class='AdContent' style='text-align:center;fond-weight:bold; width:300px;margin:25px auto;'>  <p class='wordpress'>The game is loading<a href='http://affiliate.linkwise.gr/z/140771/CD622/'><img src='http://affiliate.linkwise.gr/42/622/140771/' alt='' border='0'></a>Advertisement - click to <a href='#' onclick='hidePreRoll(); return false;'>skip</a> </p>              </div>            </div>               <div id='GameEmbed' style='position:absolute;top:0px;left:0px;right:0px;bottom:0px;visibility:hidden;'>                  <object width='450' height='321' base='.' codebase='http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000'>                    <param name='movie' value='http://assets4.playedonline.com/system/files/dn8.swf'/>                    <param name='base' value='.'/>                    <param name='quality' value='high'/>                    <param value='always' name='AllowScriptAccess'/>                    <param value='exactfit' name='scale'/>                    <embed width='450' height='321' src='http://assets4.playedonline.com/system/files/dn8.swf' type='application/x-shockwave-flash' pluginspage='http://www.macromedia.com/go/getflashplayer' quality='high' allowscriptaccess='always'/>                  </object>              </div></div><br/><br/><div style='text-align:center; font-size:smaller;'><a href='http://www.playedonline.com/game/599770/dn8.html' style='text-decoration:none;'>DN8</a> is Powered by dailygame.org</div>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_anchor_nested_img()
        {
            string input = "<a href='https://vault.fbi.gov/hottel_guy/Guy%20Hottel%20Part%201%20of%201/view?searchterm=Guy%20Hottel'><img src='http://theunitedpersons.org/wordpress/wp-content/uploads/2011/04/ufo.jpg' alt='' title='ufo' width='564' height='424' class='aligncenter size-full wp-image-7392' /></a>";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<a href='https://vault.fbi.gov/hottel_guy/Guy%20Hottel%20Part%201%20of%201/view?searchterm=Guy%20Hottel'><img src='http://theunitedpersons.org/wordpress/wp-content/uploads/2011/04/ufo.jpg' alt='' title='ufo' width='564' height='424' class='aligncenter size-full wp-image-7392' /></a>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_plain_text()
        {
            string input = "blah blah blah";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "blah blah blah";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_ignore_p_tag()
        {
            string input = "<p>blah blah blah</p>";
            string output = HtmlParser.EncloseUriLink(input);
            string expected = "<p>blah blah blah</p>";
            Assert.Equal(expected, output);
        }
    }

    public class GetImageUrls
    {
        /* not yet supported
        [Fact]
        public void Should_return_relative_urls()
        {
            HtmlParser parser = new HtmlParser("<img src=\"/Content/img/photos/anonymous/cADRobzf1U.png\" />");
            Assert.NotEmpty(parser.GetImageUris(new List<string>(){"www.insideword.com"}));
        }
         * */

        [Fact]
        public void Should_return_absolute_Url_in_domain_specified()
        {
            HtmlParser parser = new HtmlParser("<img src=\"http://www.insideword.com/Content/img/photos/anonymous/cADRobzf1U.png\" />");
            Assert.NotEmpty(parser.GetImageInfos(new List<string>() { "www.insideword.com" }));
        }
    }

    public class HtmlFree
    {
        [Fact]
        public void Should_return_only_text()
        {
            string input = "<p>innocuous text <A HREF=\"javascript:alert('hack')\">Dangerous <p>inner text</p> link</A> more <img src=\"blah\" />innocuous<span> span text</span> text<invalidtag> do not show me <p>nor me</p></invalidtag></p><p> final p</p>";
            string output = HtmlParser.HtmlFree(input);
            string expected = "innocuous text Dangerous inner text link more innocuous span text text final p";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_allow_br_tag()
        {
            string input = "this should</br> still be here";
            string output = HtmlParser.HtmlFree(input);
            string expected = "this should still be here"; // html agility pack matches open and closing tags even if the html is invalid xhtml
            Assert.Equal(expected, output);
        }

        [Fact]
        public void Should_allow_nested_tag()
        {
            string input = "<p>oooo poop we have <i>italics!</i> nested in our p tag</p>";
            string output = HtmlParser.HtmlFree(input);
            string expected = "oooo poop we have italics! nested in our p tag"; // html agility pack matches open and closing tags even if the html is invalid xhtml
            Assert.Equal(expected, output);
        }
    }
}
