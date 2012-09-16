var a2a_config = a2a_config || {
   };
a2a_config.vars = {
   vars : ["menu_type", "static_server", ["linkname", document.title || location.href], ["linkurl", location.href], "linkname_escape", ["ssl", ("https:" == document.location.protocol) ? "https://static.addtoany.com/menu" : false], "show_title", "onclick", "num_services", "hide_embeds", "prioritize", "custom_services", ["templates", {
      }
   ], "orientation", ["track_links", false], ["track_links_key", ""], "awesm", "tracking_callback", "track_pub", "color_main", "color_bg", "color_border", "color_link_text", "color_link_text_hover", "color_arrow", "color_arrow_hover", ["border_size", 8], ["localize", "", 1], ["add_services", false, 1], "no_3p", "show_menu"], process : function () {
      var j = a2a_config.vars.vars;
      for (var g = 0, k = "a2a_", d = j.length, c, f, a, l, b; g < d; g++) {
         if (typeof j[g] == "string") {
            c = j[g];
            f = window[k + c];
            l = false }
         else {
            c = j[g][0];
            f = window[k + c];
            a = j[g][1];
            l = true;
            b = j[g][2] }
         if (typeof f != "undefined" && f != null) {
            a2a_config[c] = f;
            if (!b) {
               try {
                  delete window[k + c] }
               catch (h) {
                  window[k + c] = null }
               }
            }
         else {
            if (l && !a2a_config[c]) {
               a2a_config[c] = a }
            }
         }
      }
   };
a2a_config.vars.process();
a2a_config.static_server = a2a_config.static_server || ((a2a_config.ssl) ? a2a_config.ssl : "http://static.addtoany.com/menu");
a2a_config.email_menu = (a2a_config.menu_type == "mail") ? 1 : false;
var a2a = a2a || {
   c : a2a_config, total : 0, transparent_img_url : a2a_config.static_server + "/transparent.gif", icons_img_url : a2a_config.static_server + "/icons_19.png", head_tag : document.getElementsByTagName("head")[0], init : function (c) {
      var f, b, a, d = document.createElement("div"), g, e;
      if (a2a.c.menu_type == "mail") {
         a2a.c.email_menu = 1;
         a2a.make_once() }
      c = (a2a.c.email_menu) ? "mail" : c;
      if (c) {
         a2a.type = c;
         a2a.c.vars.process() }
      window["a2a" + a2a.type + "_init"] = 1;
      f = a2a.get_all_a2a_dd();
      a2a.total += 1;
      a2a.n = a2a.total;
      a = a2a["n" + a2a.n] = {
         };
      b = a2a.set_this_index(f);
      a.type = a2a.type;
      if (b) {
         if ((!b.getAttribute("onclick") || (b.getAttribute("onclick") + "").indexOf("a2a_") == - 1) && (!b.getAttribute("onmouseover") || (b.getAttribute("onmouseover") + "").indexOf("a2a_") == - 1)) {
            if (a2a[a2a.type].onclick) {
               b.onclick = function (h) {
                  a2a.show_menu(b);
                  return false }
               }
            else {
               b.onmouseover = function () {
                  a2a.show_menu(b) };
               b.onmouseout = function () {
                  a2a.onMouseOut_delay() }
               }
            }
         }
      if (a2a.c.linkname_escape == 1) {
         e = a2a.getByClass("a2a.c.linkname_escape", f[f.length - 1])[0];
         if (e) {
            a2a.c.linkname = e.innerHTML }
         }
      d.innerHTML = a2a.c.linkname;
      g = d.childNodes[0];
      if (g) {
         a2a.c.linkname = g.nodeValue }
      delete d;
      a.linkname = a2a.c.linkname;
      a.linkurl = a2a.c.linkurl;
      a.orientation = a2a.c.orientation || false;
      a.track_links = a2a.c.track_links || false;
      a.track_links_key = a2a.c.track_links_key || "";
      a.track_pub = a2a.c.track_pub || false;
      a2a.no_brdr(f);
      a2a.c.onclick = a2a.c.linkname_escape = a2a.c.show_title = a2a.c.custom_services = a2a.c.orientation = a2a.c.num_services = a2a.c.track_pub = false;
      if (a2a.c.track_links == "custom") {
         a2a.c.track_links = false;
         a2a.c.track_links_key = "" }
      if (!a2a.locale) {
         a2a.kit();
         a2a.c.menu_type = a2a.c.email_menu = false;
         a2a.init_show() }
      }
   , init_show : function () {
      var b = a2a_config, a = a2a[a2a.type], c = a2a.show_menu;
      if (b.bookmarklet) {
         a.no_hide = 1;
         c() }
      if (b.show_menu) {
         a.no_hide = 1;
         c(false, b.show_menu) }
      }
   , get_all_a2a_dd : function () {
      return a2a.HTMLcollToArray(document.getElementsByName("a2a_dd")).concat(a2a.getByClass("a2a_dd", document)) }
   , set_this_index : function (e) {
      for (var c = 0, b = e, d = b.length, f = a2a.n; c < d; c++) {
         if (typeof b[c].a2a_index === "undefined" || b[c].a2a_index === "") {
            b[c].a2a_index = f;
            return b[c] }
         }
      }
   , get_this_index : function (d) {
      for (var c = 0, b = a2a.get_all_a2a_dd(), e = b.length; c < e; c++) {
         if (d.a2a_index == b[c].a2a_index) {
            return b[c].a2a_index }
         }
      }
   , gEl : function (a) {
      return document.getElementById(a) }
   , getByClass : function (b, c, a) {
      if (document.getElementsByClassName && Object.prototype.getElementsByClassName === document.getElementsByClassName) {
         a2a.getByClass = function (j, h, m) {
            h = h || a2a.gEl("a2a" + a2a.type + "_dropdown");
            var d = h.getElementsByClassName(j), l = (m) ? new RegExp("\\b" + m + "\\b", "i") : null, e = [], g;
            for (var f = 0, k = d.length; f < k; f += 1) {
               g = d[f];
               if (!l || l.test(g.nodeName)) {
                  e.push(g) }
               }
            return e }
         }
      else {
         if (document.evaluate) {
            a2a.getByClass = function (o, n, r) {
               r = r || "*";
               n = n || a2a.gEl("a2a" + a2a.type + "_dropdown");
               var g = o.split(" "), p = "", l = "http://www.w3.org/1999/xhtml", q = (document.documentElement.namespaceURI === l) ? l : null, h = [], d, f;
               for (var i = 0, k = g.length; i < k; i += 1) {
                  p += "[contains(concat(' ', @class, ' '), ' " + g[i] + " ')]" }
               try {
                  d = document.evaluate(".//" + r + p, n, q, 0, null) }
               catch (m) {
                  d = document.evaluate(".//" + r + p, n, null, 0, null) }
               while ((f = d.iterateNext())) {
                  h.push(f) }
               return h }
            }
         else {
            a2a.getByClass = function (r, q, u) {
               u = u || "*";
               q = q || a2a.gEl("a2a" + a2a.type + "_dropdown");
               var h = r.split(" "), t = [], d = (u === "*" && q.all) ? q.all : q.getElementsByTagName(u), p, j = [], o;
               for (var i = 0, e = h.length; i < e; i += 1) {
                  t.push(new RegExp("(^|\\s)" + h[i] + "(\\s|$)")) }
               for (var g = 0, s = d.length; g < s; g += 1) {
                  p = d[g];
                  o = false;
                  for (var f = 0, n = t.length; f < n; f += 1) {
                     o = t[f].test(p.className);
                     if (!o) {
                        break }
                     }
                  if (o) {
                     j.push(p) }
                  }
               return j }
            }
         }
      return a2a.getByClass(b, c, a) }
   , HTMLcollToArray : function (f) {
      var b = new Array(), e = f.length;
      for (var d = 0; d < e; d++) {
         b[b.length] = f[d] }
      return b }
   , add_event : function (c, b, a) {
      if (!c.addEventListener) {
         c.attachEvent("on" + b, a) }
      else {
         c.addEventListener(b, a, false) }
      }
   , onLoad : function (a) {
      var b = window.onload;
      if (typeof window.onload != "function") {
         window.onload = a }
      else {
         window.onload = function () {
            if (b) {
               b() }
            a() }
         }
      }
   , in_array : function (e, a, b) {
      if (typeof a == "object") {
         e = e.toLowerCase();
         var c = a.length;
         for (var d = 0; d < c; d++) {
            if (b) {
               if (e == a[d].toLowerCase()) {
                  return a[d] }
               }
            else {
               if (e.indexOf(a[d].toLowerCase()) != - 1 && a[d] !== "") {
                  return a[d] }
               }
            }
         }
      return false }
   , onMouseOut_delay : function () {
      var a = a2a.type;
      if (a2a.gEl("a2a" + a + "_dropdown").style.display != "none" && !a2a[a].find_focused && !a2a[a].inFocus) {
         a2a[a].delay = setTimeout("a2a.toggle_dropdown('none', '" + a + "')", 501) }
      }
   , onMouseOver_stay : function () {
      if (typeof a2a[a2a.type].delay != "undefined") {
         clearTimeout(a2a[a2a.type].delay) }
      }
   , toggle_dropdown : function (d, c) {
      if (d == "none" && a2a[c].no_hide) {
         return }
      var a, b = a2a.gEl;
      b("a2a" + c + "_dropdown").style.display = d;
      b("a2a" + c + "_border").style.display = d;
      a2a.onMouseOver_stay();
      if (d == "none") {
         a2a.embeds_visibility(true);
         if (!window.addEventListener) {
            a = document.detachEvent;
            a("onmousedown", a2a.doc_mousedown_check_scroll);
            a("onmouseup", a2a[c].doc_mouseup_toggle_dropdown) }
         else {
            document.removeEventListener("mousedown", a2a.doc_mousedown_check_scroll, false);
            document.removeEventListener("mouseup", a2a[c].doc_mouseup_toggle_dropdown, false) }
         delete a2a[c].doc_mouseup_toggle_dropdown }
      if (a2a[c].prev_keydown) {
         document.onkeydown = a2a[c].prev_keydown }
      else {
         document.onkeydown = "" }
      }
   , getPos : function (b) {
      var a = 0, c = 0;
      do {
         a += b.offsetLeft || 0;
         c += b.offsetTop || 0;
         b = b.offsetParent }
      while (b);
      return [a, c] }
   , getDocDims : function (c) {
      var a = 0, b = 0;
      if (typeof (window.innerWidth) == "number") {
         a = window.innerWidth;
         b = window.innerHeight }
      else {
         if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
            a = document.documentElement.clientWidth;
            b = document.documentElement.clientHeight }
         else {
            if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
               a = document.body.clientWidth;
               b = document.body.clientHeight }
            }
         }
      if (c == "w") {
         return a }
      else {
         return b }
      }
   , getScrollDocDims : function (c) {
      var a = 0, b = 0;
      if (typeof (window.pageYOffset) == "number") {
         a = window.pageXOffset;
         b = window.pageYOffset }
      else {
         if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
            a = document.body.scrollLeft;
            b = document.body.scrollTop }
         else {
            if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
               a = document.documentElement.scrollLeft;
               b = document.documentElement.scrollTop }
            }
         }
      if (c == "w") {
         return a }
      else {
         return b }
      }
   , show_more_less : function (d) {
      a2a.onMouseOver_stay();
      var f = a2a.gEl("a2a" + a2a.type + "_show_more_less"), g = a2a.type, a;
      if (a2a[g].show_all || d == 1) {
         a2a[g].show_all = false;
         a = (a2a.c.color_arrow == "fff") ? "_wt" : "";
         f.firstChild.className = "a2a_i_darr" + a;
         f.title = a2a.c.localize.ShowAll;
         a2a.statusbar(f, a2a.c.localize.ShowAll);
         if (d == 0) {
            a2a.default_services();
            a2a.border();
            a2a.focus_find() }
         }
      else {
         a2a[g].show_all = true;
         var c = a2a[g].main_services, b = c.length;
         for (var e = 0; e < b; e++) {
            c[e].style.display = "" }
         a = (a2a.c.color_arrow == "fff") ? "_wt" : "";
         f.firstChild.className = "a2a_i_uarr" + a;
         f.title = a2a.c.localize.ShowLess;
         a2a.statusbar(f, a2a.c.localize.ShowLess);
         a2a.border();
         a2a.focus_find() }
      a2a.embeds_visibility(false);
      if (d == 0) {
         return false }
      }
   , focus_find : function () {
      var b = a2a.gEl("a2a" + a2a.type + "_find");
      if (b.parentNode.style.display != "none") {
         b.focus() }
      }
   , blur_find : function () {
      a2a[a2a.type].find_focused = false;
      if (!a2a[a2a.type].onclick) {
         a2a.onMouseOut_delay() }
      }
   , border : function () {
      var c = a2a.gEl, e = a2a.type, f = "a2a" + e, b = c(f + "_dropdown"), a = c(f + "_border"), d = a2a.c.border_size;
      if (b.style.left && d > 0) {
         a.style.left = parseInt(b.style.left) - d + "px";
         a.style.top = parseInt(b.style.top) - d + "px";
         a.style.display = "block";
         a.style.width = (b.clientWidth || b.offsetWidth) + (d * 2) + "px";
         a.style.height = (b.clientHeight || b.offsetHeight) + (d * 2) + "px" }
      }
   , default_services : function (e) {
      var c = e || a2a.type, f = a2a[c].main_services_col_1, a = f.length, d = a2a[c].main_services_col_2, g = d.length;
      for (var b = 0; b < a; b++) {
         if (b < parseInt(a2a[c].num_services / 2)) {
            f[b].style.display = "" }
         else {
            f[b].style.display = "none" }
         }
      for (var b = 0; b < g; b++) {
         if (b < parseInt(a2a[c].num_services / 2)) {
            d[b].style.display = "" }
         else {
            d[b].style.display = "none" }
         }
      }
   , do_reset : function () {
      a2a[a2a.type].inFocus = false;
      a2a.show_more_less(1);
      a2a.border();
      a2a.focus_find();
      a2a.embeds_visibility(false) }
   , do_find : function () {
      var g = a2a.type, d = a2a[g].main_services, c = d.length, b = a2a.gEl("a2a" + g + "_find").value, f, a = a2a.in_array;
      if (b !== "") {
         terms = b.split(" ");
         for (var e = 0, h; e < c; e++) {
            h = d[e].serviceNameLowerCase;
            if (a(h, terms, false)) {
               d[e].style.display = "" }
            else {
               d[e].style.display = "none" }
            }
         }
      else {
         if (a2a[g].tab != "DEFAULT") {
            a2a.tabs(a2a[g].tab) }
         else {
            a2a.default_services() }
         }
      a2a.do_reset() }
   , tabs : function (l, i) {
      var c = a2a.getByClass("a2a_tab_selected")[0], j = a2a.type, k = a2a.gEl, b = "a2a" + j, g = k(b + "_show_more_less"), e = k(b + "_find_container"), h = k(b + "_col1"), f = k(b + "_col2"), a = k(b + "_2_col1"), n = k(b + "_2_col2"), d = "block", m = "none";
      if (c.id == "a2afeed_DEFAULT" && l == "DEFAULT") {
         return true }
      c.className = c.className.replace(/a2a_tab_selected/, "");
      k(b + "_" + l).className += " a2a_tab_selected";
      if (l != "DEFAULT") {
         g.style.display = e.style.display = h.style.display = f.style.display = m }
      else {
         g.style.display = e.style.display = h.style.display = f.style.display = d;
         a2a.default_services() }
      if (l != "EMAIL") {
         a.style.display = n.style.display = m }
      else {
         a.style.display = n.style.display = d }
      k(b + "_note_BROWSER").style.display = "none";
      k(b + "_note_EMAIL").style.display = "none";
      if (i) {
         k(b + "_note_" + l).style.display = "block" }
      a2a.do_reset();
      return false }
   , statusbar : function (a, c) {
      if (window.opera) {
         return }
      var b = a2a.gEl("a2a" + a2a.type + "_powered_by");
      if (!b.orig) {
         b.orig = b.innerHTML }
      a.onmouseover = function () {
         clearTimeout(a2a[a2a.type].statusbar_delay);
         b.innerHTML = c;
         b.style.textAlign = "left" };
      a.onmouseout = function () {
         a2a[a2a.type].statusbar_delay = setTimeout(function () {
            b.innerHTML = b.orig; b.style.textAlign = "center" }
         , 300) }
      }
   , selection : function () {
      var b, f = document.getElementsByTagName("meta"), a = f.length;
      if (window.getSelection) {
         b = window.getSelection() }
      else {
         if (document.selection) {
            b = document.selection.createRange();
            b = (b.text) ? b.text : "" }
         }
      if (b && b != "") {
         return b }
      if (a2a["n" + a2a.n].linkurl == location.href) {
         for (var c = 0, d, e; c < a; c++) {
            d = f[c].getAttribute("name");
            if (d) {
               if (d.toLowerCase() == "description") {
                  e = f[c].getAttribute("content");
                  break }
               }
            }
         }
      return (e) ? e.substring(0, 1200) : "" }
   , collections : function (c) {
      var b = a2a.gEl, a = a2a[c], d = "a2a" + c;
      a.main_services_col_1 = a2a.getByClass("a2a_i", b(d + "_col1"));
      a.main_services_col_2 = a2a.getByClass("a2a_i", b(d + "_col2"));
      a.main_services = a.main_services_col_1.concat(a.main_services_col_2);
      a.email_services = a2a.getByClass("a2a_i", b(d + "_2_col1")).concat(a2a.getByClass("a2a_i", b(d + "_2_col2")));
      a.all_services = a.main_services.concat(a.email_services) }
   , linker : function (f) {
      var e = a2a.type, b = a2a["n" + a2a.n], h = encodeURIComponent(b.linkurl).replace(/'/g, "%27"), a = encodeURIComponent(b.linkname).replace(/'/g, "%27"), d = encodeURIComponent(a2a.selection()).replace(/'/g, "%27"), g = (b.track_links && (e == "page" || e == "mail")) ? "&linktrack=" + b.track_links + "&linktrackkey=" + encodeURIComponent(b.track_links_key) : "", c = f.getAttribute("customserviceuri") || f.customserviceuri || false;
      if (c) {
         f.href = c.replace(/A2A_LINKNAME_ENC/, a).replace(/A2A_LINKURL_ENC/, h).replace(/A2A_LINKNOTE_ENC/, d) } else { f.href = "http://www.addtoany.com/add_to / " + f.safename + " ? linkurl = " + h + " & linkname = " + a + g + ((a2a.c.awesm) ? " & linktrack_parent = " + a2a.c.awesm : "") + ((f.safename == "twitter" && a2a.c.templates.twitter) ? " & template = " + encodeURIComponent(a2a.c.templates.twitter) : "") + ((e == "feed") ? " & type = feed" : "") + " & linknote = " + d } setTimeout(function () { f.href = "/" }, 500); return true }, show_menu: function (b, q) { if (b) { a2a.n = a2a.get_this_index(b) } else { a2a.n = a2a.total; a2a[a2a.type].no_hide = 1 } var g = a2a["n" + a2a.n], j = a2a.type = g.type, a = "a2a" + j; a2a.gEl(a + "_title").value = g.linkname; a2a.toggle_dropdown("block", j); var n = a2a.gEl(a + "_dropdown"), m = a2a.c.border_size, l = [n.clientWidth + (m * 2), n.clientHeight + (m * 2)], f = a2a.getDocDims("w"), p = a2a.getDocDims("h"), k = a2a.getScrollDocDims("w"), d = a2a.getScrollDocDims("h"), e, h, i, c; if (b) { e = b.getElementsByTagName("img")[0]; if (e) { h = a2a.getPos(e); i = e.clientWidth; c = e.clientHeight } else { h = a2a.getPos(b); i = b.offsetWidth; c = b.offsetHeight } if (h[0] - k + l[0] + i > f) { h[0] = h[0] - l[0] + i - 8 } if (g.orientation == "up" || g.orientation != "down" && h[1] - d + l[1] + c > p && h[1] > l[1]) { h[1] = h[1] - l[1] - c } n.style.left = m + h[0] + 2 + "px"; n.style.top = m + h[1] + c + "px" } else { if (!q) { q = {} } n.style.position = q.position || "absolute"; n.style.left = q.left || (f / 2 - l[0] / 2 + "px"); n.style.top = q.top || (p / 2 - l[1] / 2 + "px") } a2a.border(); if (!a2a[j].doc_mouseup_toggle_dropdown && !a2a[j].no_hide) { a2a.doc_mousedown_check_scroll = function () { a2a.last_scroll_pos = a2a.getScrollDocDims("h") }; a2a[j].doc_mouseup_toggle_dropdown = (function (o) { return function () { if (a2a.last_scroll_pos == a2a.getScrollDocDims("h")) { a2a.toggle_dropdown("none", o) } } })(j); if (!window.addEventListener) { document.attachEvent("onmousedown", a2a.doc_mousedown_check_scroll); document.attachEvent("onmouseup", a2a[j].doc_mouseup_toggle_dropdown) } else { document.addEventListener("mousedown", a2a.doc_mousedown_check_scroll, false); document.addEventListener("mouseup", a2a[j].doc_mouseup_toggle_dropdown, false) } document.onkeydown = a2a.checkKey } a2a.embeds_visibility(false); if (j == "feed") { a2a.gEl(a + "_DEFAULT").href = g.linkurl; if (a2a.c.fb_feedcount && !a2a.c.ssl) { a2a.feedburner_feedcount("init") } } a2a.a2a_track("test3"); if (g.track_pub) { a2a.a2a_track("z_" + g.track_pub) } }, embeds_visibility: function (h) { var b = a2a.c.hide_embeds; if (b === 0 || b === false) { return } if (!a2a.embeds) { a2a.embeds = a2a.HTMLcollToArray(document.getElementsByTagName("object")).concat(a2a.HTMLcollToArray(document.getElementsByTagName("embed"))).concat(a2a.HTMLcollToArray(document.getElementsByTagName("applet"))) } var j = a2a.gEl("a2a" + a2a.type + "_dropdown"), k = [parseInt(j.style.left), parseInt(j.style.top)], f = [j.clientWidth, j.clientHeight], g = a2a.embeds, c = g.length, e; for (var d = 0;
         d < c;
         d++) {
            e = "visible"; if (!h) {
               aPos = a2a.getPos(g[d]); aDim = [g[d].clientWidth, g[d].clientHeight]; if (k[0] < aPos[0] + aDim[0] && k[1] < aPos[1] + aDim[1] && k[0] + f[0] > aPos[0] && k[1] + f[1] > aPos[1]) {
                  e = "hidden" }
               }
            g[d].style.visibility = e }
         }
      , no_brdr : function (f) {
         var c = f, b = c.length; for (var d = 0;
         d < b;
         d++) {
            var e = c[d]; if (e.hasChildNodes() && typeof e.firstChild.style != "undefined") {
               e.firstChild.style.borderWidth = 0 }
            }
         }
      , bmBrowser : function (a) {
         var c = a2a.c.localize.Bookmark, b = a2a["n" + a2a.n]; if (document.all) {
            if (a == 1) {
               c = a2a.c.localize.AddToYourFavorites }
            else {
               window.external.AddFavorite(b.linkurl, b.linkname) }
            }
         else {
            if (a != 1) {
               a2a.gEl("a2apage_note_BROWSER").innerHTML = '<div class="a2a_note_note">' + a2a.c.localize.BookmarkInstructions + "</div>"; a2a.tabs("BROWSER", true) }
            }
         if (a == 1) {
            return c }
         }
      , loadExtScript : function (c, e, d) {
         var b = document.createElement("script"); b.charset = "UTF-8"; b.src = c; document.getElementsByTagName("head")[0].appendChild(b); if (typeof e == "function") {
            var a = setInterval(function () {
               var f = false;
               try {
                  f = e.call() }
               catch (g) {
                  }
               if (f) {
                  clearInterval(a);
                  d.call() }
               }
            , 100) }
         }
      , track : function (b) {
         var a = new Image(1, 1); a.src = b; a.width = 1; a.height = 1 }
      , a2a_track : function (o, g) {
         var q = (a2a.type != "feed") ? "Share" : "Subscribe"; if (document.cookie.length > 0) {
            var m = "__utma_a2a", s = document.cookie.indexOf(m + "="), c, p; if (s != - 1) {
               s = s + m.length + 1; c = document.cookie.indexOf(";", s); if (c == - 1) {
                  c = document.cookie.length }
               p = unescape(document.cookie.substring(s, c)) }
            }
         var k = new Date(), b = Math.round(k.getTime() / 1000), d = Math.floor(Math.random() * 9999999999), n = (p) ? p.split(".") : new Array(), r = (n[0] && n[0] != 1) ? n[0] : d, h = n[2] || b, j = n[4] || b, a = b, i = n[1] || a + 31556926, e = (n[5]) ? parseInt(n[5]) + 1 : 1, f = r + "." + i + "." + h + "." + j + "." + a + "." + e, l = "http" + ((a2a.c.ssl) ? "s" : "") + "://www.google-analytics.com/__utm.gif?&utmwv=4.6.5&a2a&utmn=" + d + "&utmhn=" + window.location.hostname + "&utmt=event&utme=5(" + q + "%20menu*" + encodeURIComponent(o) + ((g) ? "*" + encodeURIComponent(g) : "") + ")&utmcs=" + ((document.characterSet) ? document.characterSet : document.charset) + "&utmsr=" + screen.width + "x" + screen.height + "&utmsc=" + screen.colorDepth + "-bit&utmul=" + (navigator.browserLanguage || navigator.language).toLowerCase() + "&utmdt=" + document.title + "&utmhid=" + d + "&utmr=" + ((document.referrer) ? document.referrer : "-") + "&utmp=" + window.location.pathname + "&utmac=" + ((q == "Share") ? "UA-1244922-3" : "UA-1244922-4") + "&utmcc=__utma%3D" + f + "%3B%2B__utmz%3D" + r + "." + a + "." + e + ".1.utmcsr%3D(direct)%7Cutmccn%3D(direct)%7Cutmcmd%3D(none)%3B"; k.setDate(k.getDate() + 730); document.cookie = "__utma_a2a=" + escape(f) + "; expires=" + k.toGMTString() + "; path=/"; a2a.track(l) }, GA: function () { var a = a2a.type, b; a2a.onLoad(function () { if (typeof urchinTracker == "function") { b = 1 } else { if (typeof pageTracker == "object") { b = 2 } else { if (typeof _gaq == "object") { b = 3 } else { return } } } var g = a2a[a].all_services, m, d, c, j = (a == "feed") ? "subscriptions" : "pages", k = (a == "feed") ? "AddToAny Subscribe Button" : "AddToAny Share/Save Button", l, h; if (a == "page") { g.push(a2a.gEl("a2apage_any_email"), a2a.gEl("a2apage_email_client")); g = g.concat(a2a.kit_services) } for (var f = 0, e = g.length; f < e; f++) { m = g[f]; h = m.linkurl || false; c = m.getAttribute("safename") || m.safename; d = m.getAttribute("servicename") || m.servicename; if (b == 1) { l = (function (n, i, o) { return function () { urchinTracker("/addtoany.com/" + i); urchinTracker("/addtoany.com/" + i + "/" + (o || a2a["n" + a2a.n].linkurl)); urchinTracker("/addtoany.com / services / " + n) } })(c, j, h) } else { if (b == 2) { l = (function (q, n, i, o, p) { return function () { pageTracker._trackEvent(o, q, (p || a2a["n" + a2a.n].linkurl)) } })(d, c, j, k, h) } else { l = (function (q, n, i, o, p) { return function () { _gaq.push(["_trackEvent", o, q, (p || a2a["n" + a2a.n].linkurl)]) } })(d, c, j, k, h) } } a2a.add_event(m, "click", l) } }) }, kit: function () { var f = a2a.type, b = a2a.getByClass("a2a_kit", document), o = b.length, x = function (n) { for (var y = 0, z = a2a.services, t = z.length; y < t; y++) { if (n == z[y][1]) { return [z[y][0], z[y][3]] } } return false }; a2a.kit_services = a2a.kit_services || new Array(); for (var q = 0; q < o; q++) { var c = b[q], w = c.getElementsByTagName("a"), a = w.length, e = a2a["n" + a2a.n], d = document.createElement("div"); if (c.init) { continue } c.init = 1; for (var m = 0; m < a; m++) { var v = w[m], u = v.className.match(/a2a_button_([\w\.]+)(?:\s|$)/), j = (u) ? u[1] : false, l = v.childNodes, g = x(j), h = g[0], s = g[1], p, r; if (!j || !s) { continue } v.href = "http ://www.addtoany.com/add_to/" + j + "?linkurl=" + encodeURIComponent(e.linkurl).replace(/'/g, "%27") + "&type=" + f + "&linkname=" + encodeURIComponent(e.linkname).replace(/'/g, "%27") + "&linknote="; v.target = "_blank"; v.rel = "nofollow"; v.linkurl = e.linkurl; v.servicename = h; v.safename = j; if (l.length) { for (var q = 0, k = l.length; q < k; q++) { if (l[q].nodeType == 1) { r = true; break } } if (!r) { p = document.createElement("span"); p.className = "a2a_img a2a_i_" + s; v.insertBefore(p, l[0]) } } else { p = document.createElement("span"); p.className = "a2a_img a2a_i_" + s; v.appendChild(p) } a2a.kit_services.push(v) } d.style.clear = "both"; c.appendChild(d) } }, add_srvcs: function () { var g = a2a.type, h = a2a.gEl, f = h("a2a" + g + "_col1"), e = h("a2a" + g + "_col2"); if (a2a[g].custom_services) { var l = a2a[g].custom_services, a = l.length, b = a2a.mk_srvc, k = 0; l.reverse(); for (var d = 0, c; d < a; d++) { if (l[d]) { k += 1; c = b(l[d][0], l[d][0].replace(" ", "_"), false, false, false, l[d][1], l[d][2]); if (k % 2 != 0) { f.insertBefore(c, f.firstChild) } else { e.insertBefore(c, e.firstChild) } } } } if (g == "page" && a2a.c.add_services) { var l = a2a.c.add_services, a = l.length, b = a2a.mk_srvc, k = 0, j = a2a.c.ssl; for (var d = 0; d < a; d++) { if (l[d]) { k += 1; if (j) { l[d].icon = false } c = b(l[d].name, l[d].safe_name, l[d].home, false, false, false, l[d].icon); if (k % 2 != 0) { f.insertBefore(c, f.firstChild) } else { e.insertBefore(c, e.firstChild) } } } } }, prioritize_services: function () { if (!a2a.c.prioritize) { return } a2a.c.prioritize.reverse(); var h = a2a.type, j = a2a.gEl, a = "a2a" + h, g = j(a + "_col1"), f = j(a + "_col2"), b = a2a.c.prioritize, e = b.length, k = 0; for (var d = 0, c; d < e; d++) { c = j(a + "_" + b[d].toLowerCase()); if (c) { k += 1; if (k % 2 != 0) { g.insertBefore(c, g.firstChild) } else { f.insertBefore(c, f.firstChild) } } } }, user_services: function () { if (!window.postMessage || a2a.c.static_server != ((a2a.c.ssl) ? a2a.c.ssl : "http://static.addtoany.com/menu")) { a2a.history(); return } var a = a2a.type, b = document.createElement("iframe"); b.id = "a2a" + a + "_sm_ifr"; b.style.width = b.style.height = b.width = b.height = 1; b.style.top = b.style.left = b.frameborder = b.style.border = 0; b.style.position = "absolute"; b.style.zIndex = 100000; b.setAttribute("transparency", "true"); b.setAttribute("allowTransparency", "true"); b.setAttribute("frameBorder", "0"); b.src = a2a.c.static_server + "/sm1.html#" + a + ";" + location.href.split("#")[0]; if (window.postMessage && !a2a.message_event) { a2a.add_event(window, "message", function (g) { if (g.origin.substr(g.origin.length - 13) == ".addtoany.com") { var f = g.data, c = f.split("=")[1], d = f.substr(4, 4); c = (c != "") ? c.split(",") : false; a2a.gEl("a2a" + d + "_sm_ifr").style.display = "none"; a2a.history(c, d) } }); a2a.message_event = 1 } document.body.insertBefore(b, document.body.firstChild) }, history: function (r, d) { var l = d || a2a.type, c = a2a.gEl, v = c("a2a" + l + "_col1"), u = c("a2a" + l + "_col2"), y = a2a.getByClass("a2a_i", v), w = a2a.getByClass("a2a_i", u), h = new Array(), A = y.length + w.length, j = Math.abs(y.length - w.length); for (var o = 0, b = y.length - 1, a = w.length - 1; o < A + j; o++) { if (o % 2 && o != 0 && w[a]) { h[h.length] = w[a]; a-- } else { if (y[b]) { h[h.length] = y[b]; b-- } } } var n = document.createElement("div"); n.id = "a2a_hist_list"; n.style.width = "1px"; n.style.height = "1px"; n.style.overflow = "hidden"; document.body.insertBefore(n, document.body.firstChild); for (var o = 0, e = 0, g = h.length; o < g; o++) { var m = h[o], k = m.homepage, x = document.createElement("a"), p = a2a.in_array, q = false; if (!r) { x.style.clear = m.style.clear; x.href = k; x.innerHTML = k; n.appendChild(x); if (x.currentStyle) { var f = x.currentStyle.clear } else { if (document.defaultView.getComputedStyle(x, null)) { var f = document.defaultView.getComputedStyle(x, null).getPropertyValue("clear") } } x.parentNode.removeChild(x) } if (r && p(m.safename, r, true)) { q = true } if (q || (!r && f == "right" && k != "")) { m.className = m.className + " a2a_sss"; if (e % 2 && e != 0) { u.insertBefore(m, u.firstChild) } else { v.insertBefore(m, v.firstChild) } e++ } } n.parentNode.removeChild(n); a2a.collections(l); a2a.default_services(l) }, visOnly: function (f) { var b = new Array(), d = f.length; for (var c = 0, g = 0; c < d; c++) { if (f[c].style.display != "none" && f[c].parentNode.style.display != "none") { b[g] = f[c]; g++ } } return b }, moveFocus: function (a, d) { var c = a2a[a2a.type].inFocus, b = a2a.getByClass("a2a_cols"); presently_focused = a2a.visOnly(b[c[0]].getElementsByTagName("a"))[c[1]]; presently_focused.blur(); a2a[a2a.type].inFocus = [c[0] + a, c[1] + d]; c = a2a[a2a.type].inFocus; to_focus = a2a.visOnly(b[c[0]].getElementsByTagName("a"))[c[1]]; to_focus.focus(); to_focus.onblur = function () { if (this != presently_focused) { a2a[a2a.type].inFocus = false; a2a.onMouseOut_delay() } } }, checkKey: function (h) { var h = h || window.event, j = h.which || h.keyCode, c = a2a[a2a.type].inFocus, a = false, b = a2a.getByClass("a2a_cols"), k = b.length, d = a2a.visOnly; if (j == 13 && !c) { for (var g = 0; g < k; g++) { if (d(b[g].getElementsByTagName("a")).length > 0) { a = g; break } } if (a === false) { return false } a2a[a2a.type].inFocus = [a, 0]; a2a.moveFocus(0, 0); return false } if (c) { var f = d(b[c[0]].getElementsByTagName("a")); if (j == 38) { if (c[1] < 1) { if (typeof f[f.length - 1] != "undefined") { a2a.moveFocus(0, f.length - 1) } } else { if (typeof f[c[1] - 1] != "undefined") { a2a.moveFocus(0, -1) } } return false } else { if (j == 40) { if (c[1] > f.length - 2) { a2a.moveFocus(0, -(c[1])) } else { a2a.moveFocus(0, 1) } return false } else { if (j == 37) { if (c[0] < 1) { if (typeof d(b[b.length - 1].getElementsByTagName("a"))[c[1]] != "undefined") { a2a.moveFocus(b.length - 1, 0) } } else { if (typeof d(b[c[0] - 1].getElementsByTagName("a"))[c[1]] != "undefined") { a2a.moveFocus(-1, 0) } } } else { if (j == 39) { if (c[0] > b.length - 2) { if (typeof d(b[0].getElementsByTagName("a"))[c[1]] != "undefined") { a2a.moveFocus(-(b.length - 1), 0) } } else { if (typeof d(b[c[0] + 1].getElementsByTagName("a"))[c[1]] != "undefined") { a2a.moveFocus(1, 0) } } } } } } } if (j == 27 && a2a[a2a.type].tab == "DEFAULT") { a2a.gEl("a2a" + a2a.type + "_find").value = ""; a2a.do_find(); a2a.focus_find() } else { if (j > 40 || j == 32) { a2a.focus_find() } } }, css: function () { var g, p, v = a2a.c, i = v.css = document.createElement("style"), k = v.color_main || "EEE", f = v.color_bg || "FFF", j = v.color_border || "CCC", c = v.color_link_text || "00F", h = v.color_link_text_hover || "000", m = v.color_link_text_hover || "999", l = v.color_link_text || "000", q = (k.toLowerCase() == "ffffff") ? "EEE" : k, b = v.color_link_text || "CCC", e = v.color_link_text || "000", x = ".a2a_", d = "{background-position:0 ", a = "px !important}", z = x + "i_", y = a + z, w = x + "menu", u = x + "tab", t = "border", s = "background-color:", r = "color:", o = "margin:", n = "padding:"; g = "" + w + " *{" + o + "0;" + n + "0}" + w + " table{" + t + "-collapse:collapse;" + t + "-spacing:0;width:auto}" + w + " table," + w + " tbody," + w + " td," + w + " tr{" + t + ":0;" + o + "0;" + n + "0;" + s + "transparent;_" + s + "#" + f + "}" + w + " td{vertical-align:top}" + w + "," + w + "_" + t + "{display:none;z-index:9999999;position:absolute;" + o + "0;" + n + "0;-webkit-" + t + "-radius:16px;-moz-" + t + "-radius:16px}" + w + "{display:none;direction:ltr;min-width:200px;" + s + "#" + f + ";font:12px Arial,Helvetica,sans-serif;" + r + "#000;line-height:12px;" + t + ":1px solid #" + j + ";vertical-align:baseline;" + n + "8px;overflow:hidden}" + w + "_" + t + "{" + t + ":1px solid #" + k + ";" + s + "#" + k + ";opacity:.6;filter:alpha(opacity=60)}" + w + " a,#a2a_hist_list a," + u + "s div{" + r + "#" + c + ";text-decoration:none;font:12px Arial,Helvetica,sans-serif;line-height:12px;height:auto;width:auto;clear:none;outline:none;-moz-outline:none;-webkit-" + t + "-radius:8px;-moz-" + t + "-radius:8px}" + w + " a:visited,#a2a_hist_list a:visited{" + r + "#" + c + ";clear:right}" + w + " a:hover," + w + " a:active," + w + " a" + x + "i:focus," + u + "s div:hover{" + r + "#" + h + ";" + t + ":1px solid #" + j + ";" + s + "#" + k + ";text-decoration:none}" + w + " img," + w + " a img," + x + "img{width:16px;height:16px;" + t + ":0;" + n + "0;background:url(" + a2a.icons_img_url + ") no-repeat}" + w + " img{display:inline;vertical-align:middle;" + o + "0 4px 2px 0}" + w; g += " img" + x + "i_find{position:absolute;left:5px;top:2px}#a2a_menu_container{display:inline-block}#a2a_menu_container{_display:inline}" + w + "_title_container{margin-bottom:2px;" + n + "6px}" + w + "_find_container{position:relative;text-align:left;" + o + "4px 1px;" + n + "1px 24px 1px 0;" + t + ":1px solid #" + b + ";-webkit-" + t + "-radius:8px;-moz-" + t + "-radius:8px}input" + w + "_title{display:block;" + r + "#" + e + ";" + s + "#" + f + ";" + t + ":0;" + o + "0;" + n + "0;width:100%}input" + w + "_find{display:block;position:relative;left:24px;" + r + "#" + e + ";font-size:12px;" + n + "2px 0;outline:0;" + t + ":0;" + s + "transparent;_" + s + "#" + f + ";width:250px}table" + x + "cols_container{width:100%}" + x + "cols{width:50%}" + ((typeof document.body.style.maxHeight != "undefined") ? "" + x + "clear{clear:both}" : "" + x + "clear{clear:both;height:0;width:0;line-height:0;font-size:0}") + " " + x + "default_style a{float:left;" + n + "0 2px}" + x + "default_style " + x + "img{display:block;line-height:16px;overflow:hidden}" + x + "default_style " + x + "img," + x + "default_style " + x + "dd{float:left}" + x + "kit a{cursor:pointer}" + x + "hr{" + o + "0 12px 12px;" + n + "1px;background:none;" + s + "#" + k + "}" + x + "nowrap{white-space:nowrap}" + x + "note{" + o + "0 auto;" + n + "9px;font-size:11px;text-align:center}" + x + "note " + x + "note_note{" + o + "0 0 9px;" + r; g += "#" + l + "}" + x + "wide a{display:block;margin-top:3px;" + t + ":1px solid #" + q + ";" + n + "3px;text-align:center}" + u + "s{float:left;" + o + "0 0 3px}" + u + "s a," + u + "s div{" + o + "1px;" + s + "#" + k + ";" + t + ":1px solid #" + k + ";font-size:11px;" + n + "6px 12px 2px;white-space:nowrap}" + u + "s a img," + u + "s div img{margin-bottom:4px}" + u + "s a," + u + "s a:visited," + u + "s a:hover," + u + "s div," + u + "s div:hover{cursor:pointer;" + t + "-bottom:1px solid #" + k + ";" + r + "#" + h + ";-webkit-" + t + "-bottom-left-radius:0;-moz-" + t + "-radius-bottomleft:0;-webkit-" + t + "-bottom-right-radius:0;-moz-" + t + "-radius-bottomright:0}a" + u + "_selected,a" + u + "_selected:visited,a" + u + "_selected:hover,a" + u + "_selected:active,a" + u + "_selected:focus,div" + u + "_selected,div" + u + "_selected:hover{" + r + "#" + c + ";" + s + "#" + f + ";" + t + ":1px solid #" + j + ";" + t + "-bottom:1px solid #" + f + "}a" + x + "i{display:block;" + n + "4px 6px 2px;" + t + ":1px solid #" + f + ";text-align:left;white-space:nowrap}a" + x + "sss{font-weight:700}a" + x + "ind{display:inline;" + o + "0;" + n + "0}a" + x + "emailer{display:inline-block;" + t + ":1px solid #EEE;" + o + "0 9px;text-align:center}a" + w + "_show_more_less{" + o + "4px 0 8px;" + n + "0}a" + w + "_show_more_less img{vertical-align:baseline;height:12px}a" + w + "_powered_by,a" + w + "_powered_by:visited{" + s + "#" + k + ";font-size:9px;" + r; g += "#" + m + "}" + z + "a2a" + d + "0!important}" + z + "agregator" + d + "-17px!important}" + z + "aiderss" + d + "-34px!important}" + z + "aim" + d + "-51px!important}" + z + "allvoices" + d + "-68px!important}" + z + "amazon" + d + "-85px!important}" + z + "aol" + d + "-102px!important}" + z + "apple_mail" + d + "-119px!important}" + z + "arto" + d + "-136px!important}" + z + "ask" + d + "-153px!important}" + z + "avantgo" + d + "-170px!important}" + z + "backflip" + d + "-187px!important}" + z + "bebo" + d + "-204px!important}" + z + "bibsonomy" + d + "-221px!important}" + z + "bitty" + d + "-238px!important}" + z + "blinklist" + d + "-255px!important}" + z + "blogger" + d + "-272px!important}" + z + "bloglines" + d + "-289px!important}" + z + "blogmarks" + d + "-306px!important}" + z + "blogrovr" + d + "-323px!important}" + z + "bookmark" + d + "-340px!important}" + z + "bookmarks_fr" + d + "-357px!important}" + z + "box" + d + "-374px!important}" + z + "buddymarks" + d + "-391px!important}" + z + "buzmob" + d + "-408px!important}" + z + "buzz" + d + "-425px!important}" + z + "bzzster" + d + "-442px!important}" + z + "care2" + d + "-459px!important}" + z + "chrome" + d + "-476px!important}" + z + "citeulike" + d + "-493px!important}" + z + "clear" + d + "-510px!important}" + z + "connotea" + d + "-527px!important}" + z + "current" + d + "-544px!important}" + z + "dailyme" + d + "-561px!important}" + z + "dailyrotation" + d + "-578px!important}" + z; g += "darr" + d + "-595px!important}" + z + "darr_wt" + d + "-612px!important}" + z + "default" + d + "-629px!important}" + z + "delicious" + d + "-646px!important}" + z + "designfloat" + d + "-663px!important}" + z + "digg" + d + "-680px!important}" + z + "diglog" + d + "-697px!important}" + z + "diigo" + d + "-714px!important}" + z + "dzone" + d + "-731px!important}" + z + "email" + d + "-748px!important}" + z + "eskobo" + d + "-765px!important}" + z + "evernote" + d + "-782px!important}" + z + "excitemix" + d + "-799px!important}" + z + "expression" + d + "-816px!important}" + z + "facebook" + d + "-833px!important}" + z + "fark" + d + "-850px!important}" + z + "faves" + d + "-867px!important}" + z + "feed" + d + "-884px!important}" + z + "feedblitz" + d + "-901px!important}" + z + "feedbucket" + d + "-918px!important}" + z + "feedlounge" + d + "-935px!important}" + z + "feedm8" + d + "-952px!important}" + z + "feedmailer" + d + "-969px!important}" + z + "feedreader_net" + d + "-986px!important}" + z + "feedshow" + d + "-1003px!important}" + z + "find" + d + "-1020px!important}" + z + "fireant" + d + "-1037px!important}" + z + "firefox" + d + "-1054px!important}" + z + "flurry" + d + "-1071px!important}" + z + "folkd" + d + "-1088px!important}" + z + "foxiewire" + d + "-1105px!important}" + z + "friendfeed" + d + "-1122px!important}" + z + "friendster" + d + "-1139px!important}" + z + "funp" + d + "-1156px!important}" + z + "furl" + d + "-1173px!important}" + z + "fwicki" + d; g += "-1189px!important}" + z + "gabbr" + d + "-1206px!important}" + z + "global_grind" + d + "-1223px!important}" + z + "gmail" + d + "-1240px!important}" + z + "google" + d + "-1257px!important}" + z + "google_buzz" + d + "-1274px!important}" + z + "healthranker" + d + "-1291px!important}" + z + "hellotxt" + d + "-1308px!important}" + z + "hemidemi" + d + "-1325px!important}" + z + "hi5" + d + "-1342px!important}" + z + "hubdog" + d + "-1359px!important}" + z + "hugg" + d + "-1376px!important}" + z + "hyves" + d + "-1393px!important}" + z + "identica" + d + "-1410px!important}" + z + "im" + d + "-1427px!important}" + z + "imera" + d + "-1444px!important}" + z + "instapaper" + d + "-1461px!important}" + z + "iterasi" + d + "-1478px!important}" + z + "itunes" + d + "-1495px!important}" + z + "jamespot" + d + "-1512px!important}" + z + "jots" + d + "-1529px!important}" + z + "jumptags" + d + "-1546px!important}" + z + "khabbr" + d + "-1563px!important}" + z + "kledy" + d + "-1580px!important}" + z + "klipfolio" + d + "-1597px!important}" + z + "linkagogo" + d + "-1614px!important}" + z + "linkatopia" + d + "-1631px!important}" + z + "linkedin" + d + "-1648px!important}" + z + "live" + d + "-1665px!important}" + z + "livejournal" + d + "-1682px!important}" + z + "ma_gnolia" + d + "-1699px!important}" + z + "maple" + d + "-1716px!important}" + z + "meneame" + d + "-1733px!important}" + z + "mindbodygreen" + d + "-1750px!important}" + z + "miro" + d + "-1767px!important}" + z; g += "mister-wong" + d + "-1784px!important}" + z + "mixx" + d + "-1801px!important}" + z + "mobile" + d + "-1818px!important}" + z + "mozillaca" + d + "-1835px!important}" + z + "msdn" + d + "-1852px!important}" + z + "multiply" + d + "-1869px!important}" + z + "my_msn" + d + "-1886px!important}" + z + "mylinkvault" + d + "-1903px!important}" + z + "myspace" + d + "-1920px!important}" + z + "netimechannel" + d + "-1937px!important}" + z + "netlog" + d + "-1954px!important}" + z + "netomat" + d + "-1971px!important}" + z + "netvibes" + d + "-1988px!important}" + z + "netvouz" + d + "-2005px!important}" + z + "newgie" + d + "-2022px!important}" + z + "newsalloy" + d + "-2039px!important}" + z + "newscabby" + d + "-2056px!important}" + z + "newsgator" + d + "-2073px!important}" + z + "newshutch" + d + "-2090px!important}" + z + "newsisfree" + d + "-2107px!important}" + z + "newstrust" + d + "-2124px!important}" + z + "newsvine" + d + "-2141px!important}" + z + "nowpublic" + d + "-2158px!important}" + z + "odeo" + d + "-2175px!important}" + z + "oneview" + d + "-2192px!important}" + z + "openbm" + d + "-2209px!important}" + z + "orkut" + d + "-2226px!important}" + z + "outlook" + d + "-2243px!important}" + z + "pageflakes" + d + "-2260px!important}" + z + "pdf" + d + "-2277px!important}" + z + "phonefavs" + d + "-2294px!important}" + z + "ping" + d + "-2311px!important}" + z + "plaxo" + d + "-2328px!important}" + z + "plurk" + d + "-2345px!important}" + z + "plusmo" + d; g += "-2362px!important}" + z + "podnova" + d + "-2379px!important}" + z + "posterous" + d + "-2396px!important}" + z + "print" + d + "-2413px!important}" + z + "printfriendly" + d + "-2430px!important}" + z + "propeller" + d + "-2447px!important}" + z + "protopage" + d + "-2464px!important}" + z + "pusha" + d + "-2481px!important}" + z + "rapidfeeds" + d + "-2498px!important}" + z + "rasasa" + d + "-2515px!important}" + z + "reader" + d + "-2532px!important}" + z + "reddit" + d + "-2549px!important}" + z + "rssfwd" + d + "-2566px!important}" + z + "segnalo" + d + "-2583px!important}" + z + "share" + d + "-2600px!important}" + z + "shoutwire" + d + "-2617px!important}" + z + "shyftr" + d + "-2634px!important}" + z + "simpy" + d + "-2651px!important}" + z + "sitejot" + d + "-2668px!important}" + z + "skimbit" + d + "-2685px!important}" + z + "slashdot" + d + "-2702px!important}" + z + "smaknews" + d + "-2719px!important}" + z + "sodahead" + d + "-2736px!important}" + z + "sofomo" + d + "-2753px!important}" + z + "spaces" + d + "-2770px!important}" + z + "sphere" + d + "-2787px!important}" + z + "sphinn" + d + "-2803px!important}" + z + "spurl" + d + "-2820px!important}" + z + "squidoo" + d + "-2837px!important}" + z + "startaid" + d + "-2854px!important}" + z + "strands" + d + "-2871px!important}" + z + "stumbleupon" + d + "-2888px!important}" + z + "stumpedia" + d + "-2905px!important}" + z + "symbaloo" + d + "-2922px!important}" + z + "taggly" + d + "-2939px!important}" + z; g += "tagza" + d + "-2956px!important}" + z + "tailrank" + d + "-2973px!important}" + z + "technet" + d + "-2990px!important}" + z + "technorati" + d + "-3007px!important}" + z + "technotizie" + d + "-3024px!important}" + z + "thefreedictionary" + d + "-3041px!important}" + z + "thefreelibrary" + d + "-3058px!important}" + z + "thunderbird" + d + "-3075px!important}" + z + "tipd" + d + "-3092px!important}" + z + "toolbar_google" + d + "-3109px!important}" + z + "tumblr" + d + "-3126px!important}" + z + "twiddla" + d + "-3143px!important}" + z + "twine" + d + "-3160px!important}" + z + "twitter" + d + "-3177px!important}" + z + "txtvox" + d + "-3194px!important}" + z + "typepad" + d + "-3211px!important}" + z + "uarr" + d + "-3228px!important}" + z + "uarr_wt" + d + "-3245px!important}" + z + "unalog" + d + "-3262px!important}" + z + "viadeo" + d + "-3279px!important}" + z + "webnews" + d + "-3296px!important}" + z + "webwag" + d + "-3314px!important}" + z + "wikio" + d + "-3331px!important}" + z + "windows_mail" + d + "-3348px!important}" + z + "wink" + d + "-3365px!important}" + z + "winksite" + d + "-3382px!important}" + z + "wists" + d + "-3399px!important}" + z + "wordpress" + d + "-3416px!important}" + z + "xanga" + d + "-3433px!important}" + z + "xerpi" + d + "-3450px!important}" + z + "xianguo" + d + "-3467px!important}" + z + "yahoo" + d + "-3484px!important}" + z + "yample" + d + "-3501px!important}" + z + "yigg" + d + "-3518px!important}" + z + "yim" + d; g += "-3535px!important}" + z + "yoolink" + d + "-3552px!important}" + z + "youmob" + d + "-3569px!important}" + z + "yourminis" + d + "-3586px!important}" + z + "zaptxt" + d + "-3603px!important}" + z + "zhuaxia" + d + "-3620px!important}" + z + "zune" + d + "-3637px; }"; i.setAttribute("type", "text/css"); if (i.styleSheet) { i.styleSheet.cssText = g } else { p = document.createTextNode(g); i.appendChild(p) } a2a.head_tag.appendChild(i) }, mk_srvc: function (a, c, f, h, g, i, d) { var j = document.createElement("a"), b = a2a.c, e = function () { a2a.linker(this, this.safename) }; j.id = "a2a" + a2a.type + "_" + c; j.rel = "nofollow"; j.className = "a2a_i"; j.href = "/"; j.target = "_blank"; j.onmousedown = e; j.onkeydown = e; j.homepage = f; j.safename = c; j.servicename = a; j.serviceNameLowerCase = a.toLowerCase(); j.appendChild(b.transparent_img.cloneNode(true)); j.appendChild(document.createTextNode(a)); a2a.add_event(j, "click", function () { var k = a2a["n" + a2a.n]; a2a.a2a_track("testShare1", a); if (k.track_pub) { a2a.a2a_track("z_" + k.track_pub + "Share", a) } }); if (g) { j.stype = g } if (b.tracking_callback) { a2a.add_event(j, "click", function () { var k = a2a["n" + a2a.n], l = { service: a, title: k.linkname, url: k.linkurl }; b.tracking_callback(l) }) } if (i) { j.customserviceuri = i } if (d) { j.firstChild.style.backgroundImage = "url(" + d + ")" } else { if (h) { j.firstChild.className = "a2a_i_" + h } else { j.firstChild.className = "a2a_i_default" } } return j }, i18n: function () { var c = ["ar", "id", "bn", "bs", "bg", "ca", "ca-AD", "ca-ES", "cs", "da", "de", "el", "et", "es", "es-AR", "es-VE", "eo", "en-US", "fa", "fr", "fr-CA", "he-IL", "hi", "hr", "is", "it", "ja", "ko-KR", "lv", "lt", "hu", "mk", "nl", "no", "pl-PL", "pt", "pt-BR", "pt-PT", "ro", "ru", "sr", "fi", "sk", "sl", "sv-SE", "ta", "te", "tr-TR", "uk", "vi", "zh-CN", "zh-TW"], d = (navigator.browserLanguage || navigator.language).toLowerCase(), b = a2a.in_array(d, c, true); if (!b) { var a = d.indexOf("-"); if (a != -1) { b = a2a.in_array(d.substr(0, a), c, true) } } if (d != "en-us" && b) { return b } else { return false } } }; a2a.make_once = function () { a2a.type = a2a.c.menu_type || "page"; if (!a2a[a2a.type] && !window["a2a" + a2a.type + "_init"]) { a2a[a2a.type] = {}; window.a2a_show_dropdown = a2a.show_menu; window.a2a_onMouseOut_delay = a2a.onMouseOut_delay; window.a2a_fluids = a2a.border; window.a2a_init = a2a.init; a2a.create_page_dropdown = function (z) { var g = a2a.gEl, l = a2a.type = z, j = "a2a" + l, y = a2a.c, x = y.localize, t = y.transparent_img = new Image(), v = a2a.transparent_img_url; a2a.css(); x = y.localize = { Share: x.Share || "Share", Save: x.Save || "Save", Subscribe: x.Subscribe || "Subscribe", Email: x.Email || "E-mail", Bookmark: x.Bookmark || "Bookmark", ShowAll: x.ShowAll || "Show all", ShowLess: x.ShowLess || "Show less", FindAnyServiceToAddTo: x.FindAnyServiceToAddTo || "Instantly find any service to add to", PoweredBy: x.PoweredBy || "Powered by", AnyEmail: "Any e-mail", ShareViaEmail: x.ShareViaEmail || "Share via e-mail", SubscribeViaEmail: x.SubscribeViaEmail || "Subscribe via email", BookmarkInYourBrowser: x.BookmarkInYourBrowser || "Bookmark in your browser", BookmarkInstructions: x.BookmarkInstructions || "Press Ctrl+D or &#8984;+D to bookmark this page", AddToYourFavorites: x.AddToYourFavorites || "Add to Favorites", SendFromWebOrProgram: x.SendFromWebOrProgram || "Send from any other e-mail address or e-mail program", EmailProgram: x.EmailProgram || "E-mail program" }; var h = '<div id="a2a' + l + '_border" class="a2a_menu_border" onmouseover="a2a.onMouseOver_stay()"' + ((a2a[l].onclick) ? "" : ' onmouseout="a2a.onMouseOut_delay()"') + '></div><div id="a2a' + l + '_dropdown" class="a2a_menu" onmouseover="a2a.onMouseOver_stay()"' + ((a2a[l].onclick) ? "" : ' onmouseout="a2a.onMouseOut_delay()"') + '><table><tr><td><div id="a2a' + l + '_title_container" class="a2a_menu_title_container"' + ((a2a[l].show_title) ? "" : ' style="display:none"') + '><input id="a2a' + l + '_title" class="a2a_menu_title"/></div>'; if (l == "page") { h += '<div class="a2a' + l + '_wide a2a_wide"><div class="a2a_tabs"><div id="a2a' + l + '_DEFAULT" class="a2a_tab_selected" style="margin-right:1px" onclick="return a2a.tabs(\'DEFAULT\')"><img class="a2a_i_share" src="' + v + '"/>' + x.Share + " / " + x.Save + '</div></div><div class="a2a_tabs"><div title="' + x.ShareViaEmail + '" id="a2a' + l + '_EMAIL" style="margin-right:1px" onclick="return a2a.tabs(\'EMAIL\',true)"><img class="a2a_i_email" src="' + v + '"/>' + x.Email + '</div></div><div class="a2a_tabs"><div onclick="a2a.bmBrowser()" title="' + x.BookmarkInYourBrowser + '" id="a2a' + l + '_BROWSER" style="margin-left:1px"><img class="a2a_i_bookmark" src="' + v + '"/>' + a2a.bmBrowser(1) + '</div></div></div><div class="a2a_clear"></div>' } if (l == "page") { h += '<div id="a2a' + l + '_find_container" class="a2a_menu_find_container"><input id="a2a' + l + '_find" class="a2a_menu_find" type="text" onclick="a2a.focus_find()" onkeyup="a2a.do_find()" autocomplete="off" onfocus="a2a[\'' + l + '\'].find_focused=true;a2a.onMouseOver_stay()" onblur="a2a.blur_find()" title="' + x.FindAnyServiceToAddTo + '"><img id="a2a' + l + '_find_icon" class="a2a_i_find" src="' + v + '" onclick="a2a.focus_find()"/></div>' } h += '<table id="a2a' + l + '_cols_container" class="a2a_cols_container"><tr><td class="a2a_cols"><div id="a2a' + l + '_col1"' + ((l == "mail") ? ' style="display:none"' : "") + '></div><div id="a2a' + l + '_2_col1"' + ((l != "mail") ? ' style="display:none"' : "") + '></div></td><td class="a2a_cols"><div id="a2a' + l + '_col2"' + ((l == "mail") ? ' style="display:none"' : "") + '></div><div id="a2a' + l + '_2_col2"' + ((l != "mail") ? ' style="display:none"' : "") + '></div></td></tr></table><div id="a2a' + l + '_note_BROWSER" class="a2a_note" style="display:none"></div><div id="a2a' + l + '_note_EMAIL" class="a2a_note"' + ((l != "mail") ? ' style="display:none"' : "") + '><div class="a2a_hr"></div><div class="a2a_note_note">' + x.SendFromWebOrProgram + ':</div><div class="a2a_nowrap"><a href="/" id="a2a' + l + '_any_email" class="a2a_i a2a_emailer" target="_blank" servicename="Email (form)" safename="email_form" customserviceuri="http://www.addtoany.com/email?linkurl=A2A_LINKURL_ENC&amp;linkname=A2A_LINKNAME_ENC" onkeydown="a2a.linker(this)" onmousedown="a2a.linker(this)" onmouseup="a2a.a2a_track(\'testShare1\', \'Email\');if(a2a.c.track_pub)a2a.a2a_track(\'z_\'+a2a.c.track_pub+\'Share\', \'Email\')" style="margin-right:9px"><img class="a2a_i_email" src="' + v + '"/>' + x.AnyEmail + '</a><a href="/" class="a2a_i a2a_emailer" id="a2a' + l + '_email_client" servicename="Email (mailto)" safename="email_mailto" customserviceuri="mailto:?subject=A2A_LINKNAME_ENC&amp;body=A2A_LINKURL_ENC" onkeydown="a2a.linker(this)" onmousedown="a2a.linker(this)" onmouseup="a2a.a2a_track(\'testShare1\', \'Email\');if(a2a.c.track_pub)a2a.a2a_track(\'z_\'+a2a.c.track_pub+\'Share\', \'Email\')" style="margin-left:9px"><img class="a2a_i_outlook" src="' + v + '"/><img class="a2a_i_windows_mail" src="' + v + '"/><img class="a2a_i_apple_mail" src="' + v + '"/><img class="a2a_i_thunderbird" src="' + v + '"/></a></div></div>'; if (l != "mail") { h += '<div class="a2a' + l + '_wide a2a_wide"><a href="javascript:void(0)" id="a2a' + l + "_show_more_less\" class=\"a2a_menu_show_more_less\" onClick=\"return a2a.show_more_less(0)\" onmouseover=\"img=this.firstChild;if(a2a.c.color_arrow_hover=='fff'){if(img.className.indexOf('_wt')==-1)img.className+='_wt'}else img.className=img.className.replace(/_wt/,'')\" onmouseout=\"img=this.firstChild;if(a2a.c.color_arrow=='fff'){if(img.className.indexOf('_wt')==-1)img.className+='_wt'}else img.className=img.className.replace(/_wt/,'')\" title=\"" + x.ShowAll + '"><img class="a2a_i_darr' + ((y.color_arrow == "fff") ? "_wt" : "") + '" src="' + v + '"/></a></div>' } h += '<div class="a2a' + l + '_wide a2a_wide"><a href="http://www.addtoany.com/" id="a2a' + l + '_powered_by" class="a2a_menu_powered_by" target="_blank" title="Share &amp; Subscribe buttons" onmouseover="if(!window.opera)this.innerHTML=this.orig;this.style.textAlign=\'center\'">' + x.PoweredBy + " AddToAny</a></div></td></tr></table></div>"; var s = "a2a_menu_container"; a2a_w = g(s) || document.createElement("div"), stop_propagation = function (i) { if (!i) { i = window.event } i.cancelBubble = true; if (i.stopPropagation) { i.stopPropagation() } }; a2a_w.onmouseup = a2a_w.onmousedown = stop_propagation; a2a_w.innerHTML = h; if (a2a_w.id != s) { a2a_w.style.position = "static"; document.body.insertBefore(a2a_w, document.body.firstChild) } else { y.border_size = 0 } t.src = v; t.width = 16; t.height = 16; var k = new RegExp("[\\?&]awesm=([^&#]*)"), o = k.exec(window.location.href); if (o != null) { y.awesm = o[1] } else { y.awesm = false } var m = a2a.mk_srvc, n = { most: {}, email: {} }; n.most.col1 = new Array(["Facebook", "facebook", "http://www.facebook.com/", "facebook"], ["Delicious", "delicious", "http://delicious.com/", "delicious"], ["Google Bookmarks", "google_bookmarks", "http://www.google.com/bookmarks", "google"], ["MySpace", "myspace", "http://www.myspace.com/", "myspace"], ["Yahoo Buzz", "yahoo_buzz", "http://buzz.yahoo.com/", "buzz"], ["StumbleUpon", "stumbleupon", "http://www.stumbleupon.com/", "stumbleupon"], ["Bebo", "bebo", "http://www.bebo.com/", "bebo"], ["WordPress", "wordpress", "http://wordpress.com/", "wordpress"], ["Orkut", "orkut", "http://www.orkut.com/", "orkut"], ["Netvibes Share", "netvibes_share", "http://www.netvibes.com/", "netvibes"], ["Strands", "strands", "http://www.strands.com/", "strands"], ["DailyMe", "dailyme", "http://dailyme.com/", "dailyme"], ["TechNet", "technet", "http://social.technet.microsoft.com/", "technet"], ["Arto", "arto", "http://www.arto.com/", "arto"], ["SmakNews", "smaknews", "http://smaknews.com/", "smaknews"], ["AIM", "aim", "http://www.aim.com/", "aim"], ["Identi.ca", "identi_ca", "http://identi.ca/", "identica"], ["Blogger Post", "blogger_post", "http://www.blogger.com/", "blogger"], ["Box.net", "box_net", "https://www.box.net/", "box"], ["Netlog", "netlog", "http://www.netlog.com/", "netlog"], ["Shoutwire", "shoutwire", "http://www.shoutwire.com/", "shoutwire"], ["Jumptags", "jumptags", "http://www.jumptags.com/", "jumptags"], ["Hemidemi", "hemidemi", "http://www.hemidemi.com/", "hemidemi"], ["Instapaper", "instapaper", "http://www.instapaper.com/", "instapaper"], ["Xerpi", "xerpi", "http://www.xerpi.com/", "xerpi"], ["Wink", "wink", "http://www.wink.com/", "wink"], ["BibSonomy", "bibsonomy", "http://www.bibsonomy.org/", "bibsonomy"], ["BlogMarks", "blogmarks", "http://blogmarks.net/", "blogmarks"], ["StartAid", "startaid", "http://www.startaid.com/", "startaid"], ["Khabbr", "khabbr", "http://www.khabbr.com/", "khabbr"], ["Yoolink", "yoolink", "http://www.yoolink.fr/", "yoolink"], ["Technotizie", "technotizie", "http://www.technotizie.it/", "technotizie"], ["Multiply", "multiply", "http://multiply.com/", "multiply"], ["Plaxo Pulse", "plaxo_pulse", "http://pulse.plaxo.com/pulse/", "plaxo"], ["Squidoo", "squidoo", "http://www.squidoo.com/", "squidoo"], ["Blinklist", "blinklist", "http://www.blinklist.com/", "blinklist"], ["YiGG", "yigg", "http://www.yigg.de/", "yigg"], ["Segnalo", "segnalo", "http://segnalo.alice.it/", "segnalo"], ["YouMob", "youmob", "http://youmob.com/", "youmob"], ["Fark", "fark", "http://www.fark.com/", "fark"], ["Jamespot", "jamespot", "http://www.jamespot.com/", "jamespot"], ["Twiddla", "twiddla", "http://www.twiddla.com/", "twiddla"], ["MindBodyGreen", "mindbodygreen", "http://www.mindbodygreen.com/", "mindbodygreen"], ["Hugg", "hugg", "http://www.hugg.com/", "hugg"], ["NowPublic", "nowpublic", "http://www.nowpublic.com/", "nowpublic"], ["Tumblr", "tumblr", "http://www.tumblr.com/", "tumblr"], ["Current", "current", "http://current.com/", "current"], ["Spurl", "spurl", "http://www.spurl.net/", "spurl"], ["Oneview", "oneview", "http://www.oneview.de/", "oneview"], ["Simpy", "simpy", "http://www.simpy.com/", "simpy"], ["BuddyMarks", "buddymarks", "http://www.buddymarks.com/", "buddymarks"], ["Viadeo", "viadeo", "http://www.viadeo.com/", "viadeo"], ["Wists", "wists", "http://www.wists.com/", "wists"], ["Backflip", "backflip", "http://www.backflip.com/", "backflip"], ["SiteJot", "sitejot", "http://www.sitejot.com/", "sitejot"], ["Health Ranker", "health_ranker", "http://www.healthranker.com/", "healthranker"], ["Care2 News", "care2_news", "http://www.care2.com/news/", "care2"], ["Sphere", "sphere", "http://www.sphere.com/", "sphere"], ["Gabbr", "gabbr", "http://www.gabbr.com/", "gabbr"], ["Tagza", "tagza", "http://www.tagza.com/", "tagza"], ["Folkd", "folkd", "http://www.folkd.com/", "folkd"], ["NewsTrust", "newstrust", "http://newstrust.net/", "newstrust"], ["PrintFriendly", "printfriendly", "http://www.printfriendly.com", "printfriendly"]); n.email.col1 = new Array(["Yahoo Mail", "yahoo_mail", "", "yahoo", "email"], ["AOL Mail", "aol_mail", "", "aol", "email"]); n.most.col2 = new Array(["Twitter", "twitter", "http://twitter.com/", "twitter"], ["Digg", "digg", "http://digg.com/", "digg"], ["Google Buzz", "google_buzz", "http://mail.google.com/mail/", "google_buzz"], ["Reddit", "reddit", "http://www.reddit.com/", "reddit"], ["Windows Live Favorites", "windows_live_favorites", "http://favorites.live.com/", "live"], ["Yahoo Bookmarks", "yahoo_bookmarks", "http://bookmarks.yahoo.com/", "yahoo"], ["Mister-Wong", "mister_wong", "http://www.mister-wong.com/", "mister-wong"], ["Google Reader", "google_reader", "http://www.google.com/reader/", "reader"], ["Evernote", "evernote", "http://www.evernote.com/", "evernote"], ["Stumpedia", "stumpedia", "http://www.stumpedia.com/", "stumpedia"], ["Posterous", "posterous", "http://posterous.com", "posterous"], ["MSDN", "msdn", "http://social.msdn.microsoft.com/", "msdn"], ["Expression", "expression", "http://social.expression.microsoft.com/", "expression"], ["Tipd", "tipd", "http://tipd.com/", "tipd"], ["Plurk", "plurk", "http://www.plurk.com/", "plurk"], ["Yahoo Messenger", "yahoo_messenger", "http://messenger.yahoo.com/", "yim"], ["Mozillaca", "mozillaca", "http://www.mozillaca.com/", "mozillaca"], ["TypePad Post", "typepad_post", "http://www.typepad.com/", "typepad"], ["Mixx", "mixx", "http://mixx.com/", "mixx"], ["Technorati Favorites", "technorati_favorites", "http://technorati.com/", "technorati"], ["CiteULike", "citeulike", "http://www.citeulike.org/", "citeulike"], ["Windows Live Spaces", "windows_live_spaces", "http://spaces.live.com/", "spaces"], ["FunP", "funp", "http://funp.com/", "funp"], ["PhoneFavs", "phonefavs", "http://phonefavs.com/", "phonefavs"], ["Netvouz", "netvouz", "http://www.netvouz.com/", "netvouz"], ["Diigo", "diigo", "http://www.diigo.com/", "diigo"], ["Taggly", "taggly", "http://www.taggly.com/", "taggly"], ["Tailrank", "tailrank", "http://www.tailrank.com/", "tailrank"], ["Kledy", "kledy", "http://www.kledy.de/", "kledy"], ["Meneame", "meneame", "http://meneame.net/", "meneame"], ["Bookmarks.fr", "bookmarks_fr", "http://www.bookmarks.fr/", "bookmarks_fr"], ["NewsVine", "newsvine", "http://www.newsvine.com/", "newsvine"], ["FriendFeed", "friendfeed", "http://friendfeed.com/", "friendfeed"], ["Ping", "ping", "http://ping.fm/", "ping"], ["Protopage Bookmarks", "protopage_bookmarks", "http://www.protopage.com/", "protopage"], ["Faves", "faves", "http://faves.com/", "faves"], ["Webnews", "webnews", "http://www.webnews.de/", "webnews"], ["Pusha", "pusha", "http://www.pusha.se/", "pusha"], ["Slashdot", "slashdot", "http://slashdot.org/", "slashdot"], ["Allvoices", "allvoices", "http://www.allvoices.com/", "allvoices"], ["Imera Brazil", "imera_brazil", "http://imera.com.br/", "imera"], ["LinkaGoGo", "linkagogo", "http://www.linkagogo.com/", "linkagogo"], ["unalog", "unalog", "http://unalog.com/", "unalog"], ["Diglog", "diglog", "http://www.diglog.com/", "diglog"], ["Propeller", "propeller", "http://www.propeller.com/", "propeller"], ["LiveJournal", "livejournal", "http://www.livejournal.com/", "livejournal"], ["HelloTxt", "hellotxt", "http://hellotxt.com", "hellotxt"], ["Yample", "yample", "http://yample.com/", "yample"], ["Linkatopia", "linkatopia", "http://www.linkatopia.com/", "linkatopia"], ["LinkedIn", "linkedin", "http://www.linkedin.com/", "linkedin"], ["Ask.com MyStuff", "ask_com_mystuff", "http://mystuff.ask.com/", "ask"], ["Maple", "maple", "http://www.maple.nu/", "maple"], ["Connotea", "connotea", "http://www.connotea.org/", "connotea"], ["MyLinkVault", "mylinkvault", "http://www.mylinkvault.com/", "mylinkvault"], ["Sphinn", "sphinn", "http://sphinn.com/", "sphinn"], ["DZone", "dzone", "http://www.dzone.com/", "dzone"], ["Hyves", "hyves", "http://www.hyves.nl/", "hyves"], ["Bitty Browser", "bitty_browser", "http://www.bitty.com/", "bitty"], ["Symbaloo Feeds", "symbaloo_feeds", "http://www.symbaloo.com/", "symbaloo"], ["Foxiewire", "foxiewire", "http://www.foxiewire.com/", "foxiewire"], ["VodPod", "vodpod", "http://vodpod.com/", "default"], ["Amazon Wish List", "amazon_wish_list", "http://www.amazon.com/", "amazon"], ["Read It Later", "read_it_later", "http://readitlaterlist.com/", "default"]); n.email.col2 = new Array(["Google Gmail", "google_gmail", "", "gmail", "email"], ["Hotmail", "hotmail", "", "live", "email"]); for (var p = 1; p < 3; p++) { if (l != "mail") { for (var w = 0, f = n.most["col" + p], r = f.length; w < r; w++) { var u = f[w]; g(j + "_col" + p).appendChild(m(u[0], u[1], u[2], u[3], u[4])) } } for (var w = 0, e = n.email["col" + p], q = e.length; w < q; w++) { var u = e[w]; g(j + "_2_col" + p).appendChild(m(u[0], u[1], u[2], u[3], u[4])) } } a2a.services = n.most.col1.concat(n.most.col2).concat(n.email.col1.concat(n.email.col2)); if (l == "page") { a2a.statusbar(g(j + "_DEFAULT"), x.Share + " / " + x.Save); a2a.statusbar(g(j + "_EMAIL"), x.ShareViaEmail); a2a.statusbar(g(j + "_BROWSER"), x.BookmarkInYourBrowser) } a2a.statusbar(g(j + "_email_client"), x.EmailProgram); if (l == "page") { a2a.statusbar(g(j + "_show_more_less"), x.ShowAll); a2a.statusbar(g(j + "_find"), x.FindAnyServiceToAddTo) } a2a.prioritize_services(); a2a.add_srvcs(); a2a.user_services(); a2a.collections(l); a2a.default_services(); a2a.GA() }; var c = a2a.type, a = a2a[c], b = a2a.c; a.find_focused = false; a.show_all = false; a.inFocus = false; a.prev_keydown = document.onkeydown || false; a.tab = "DEFAULT"; a.onclick = b.onclick || false; a.show_title = b.show_title || false; a.num_services = b.num_services || 12; a.custom_services = b.custom_services || false; a2a.locale = a2a.i18n(); if (a2a.locale && b.static_server == ((b.ssl) ? b.ssl : "http://static.addtoany.com/menu")) { a2a.loadExtScript(b.static_server + "/locale/" + a2a.locale + ".js", function () { return (a2a_localize != "") }, function () { b.localize = a2a_localize; b.add_services = window.a2a_add_services; a2a.create_page_dropdown(c); a2a.kit(); a2a.init_show() }); b.menu_type = b.email_menu = false } else { a2a.create_page_dropdown(c) } try { document.execCommand("BackgroundImageCache", false, true) } catch (d) { } a2a.a2a_track("TestHit1"); if (!b.ssl && !b.no_3p) { a2a.track("http://map.media6degrees.com/orbserv/hbpix?pixId=2869&curl=" + encodeURIComponent(location.href)) } } }; a2a.make_once(); a2a.init();