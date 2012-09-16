// Function retrieved from: http://cass- hacks.com/articles/code/js_url_encode_decode/
function URLEncode (clearString) {
  var output = '';
  var x = 0;
  clearString = clearString.toString();
  var regex = /(^[a-zA-Z0-9_.]*)/;
  while (x < clearString.length) {
    var match = regex.exec(clearString.substr(x));
    if (match != null && match.length > 1 && match[1] != '') {
        output += match[1];
      x += match[1].length;
    } else {
      if (clearString[x] == ' ')
        output += '+';
      else {
        var charCode = clearString.charCodeAt(x);
        var hexVal = charCode.toString(16);
        output += '%' + ( hexVal.length < 2 ? '0' : '' ) + hexVal.toUpperCase();
      }
      x++;
    }
  }
  return output;
}

// Retrieved from: http://www.htmlcodetutorial.com/forms/index_famsupp_157.html
function SubmitOnEnter(myfield, e)
{
    var keycode;
    if (window.event)
        keycode = window.event.keyCode;
    else if (e)
        keycode = e.which;
    else
        return true;
    if (keycode == 13)
    {
        document.location.href = 'http://www.insideword.com/info/search?cx=004872866898364041263:qvgddu_rzok&cof=FORID:9&ie=UTF-8&q=' + URLEncode(document.getElementById('q').value) + '&sa=Search';
        return false;
    }
    else
        return true;
 }