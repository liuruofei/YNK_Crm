!function(){"use strict";function t(t){f.push(t),1==f.length&&l()}function e(){for(;f.length;)f[0](),f.shift()}function n(t){this.a=d,this.b=void 0,this.f=[];var e=this;try{t(function(t){r(e,t)},function(t){s(e,t)})}catch(n){s(e,n)}}function i(t){return new n(function(e,n){n(t)})}function o(t){return new n(function(e){e(t)})}function r(t,e){if(t.a==d){if(e==t)throw new TypeError;var n=!1;try{var i=e&&e.then;if(null!=e&&"object"==typeof e&&"function"==typeof i)return void i.call(e,function(e){n||r(t,e),n=!0},function(e){n||s(t,e),n=!0})}catch(o){return void(n||s(t,o))}t.a=0,t.b=e,a(t)}}function s(t,e){if(t.a==d){if(e==t)throw new TypeError;t.a=1,t.b=e,a(t)}}function a(e){t(function(){if(e.a!=d)for(;e.f.length;){var t=e.f.shift(),n=t[0],i=t[1],o=t[2],t=t[3];try{0==e.a?o("function"==typeof n?n.call(void 0,e.b):e.b):1==e.a&&("function"==typeof i?o(i.call(void 0,e.b)):t(e.b))}catch(r){t(r)}}})}function c(t){return new n(function(e,n){function i(n){return function(i){s[n]=i,r+=1,r==t.length&&e(s)}}var r=0,s=[];0==t.length&&e(s);for(var a=0;a<t.length;a+=1)o(t[a]).c(i(a),n)})}function u(t){return new n(function(e,n){for(var i=0;i<t.length;i+=1)o(t[i]).c(e,n)})}var l,f=[];l=function(){setTimeout(e)};var d=2;n.prototype.g=function(t){return this.c(void 0,t)},n.prototype.c=function(t,e){var i=this;return new n(function(n,o){i.f.push([t,e,n,o]),a(i)})},window.Promise||(window.Promise=n,window.Promise.resolve=o,window.Promise.reject=i,window.Promise.race=u,window.Promise.all=c,window.Promise.prototype.then=n.prototype.c,window.Promise.prototype["catch"]=n.prototype.g)}(),function(){function t(t,e){u?t.addEventListener("scroll",e,!1):t.attachEvent("scroll",e)}function e(t){document.body?t():u?document.addEventListener("DOMContentLoaded",t):document.attachEvent("onreadystatechange",function(){"interactive"!=document.readyState&&"complete"!=document.readyState||t()})}function n(t){this.a=document.createElement("div"),this.a.setAttribute("aria-hidden","true"),this.a.appendChild(document.createTextNode(t)),this.b=document.createElement("span"),this.c=document.createElement("span"),this.h=document.createElement("span"),this.f=document.createElement("span"),this.g=-1,this.b.style.cssText="max-width:none;display:inline-block;position:absolute;height:100%;width:100%;overflow:scroll;font-size:16px;",this.c.style.cssText="max-width:none;display:inline-block;position:absolute;height:100%;width:100%;overflow:scroll;font-size:16px;",this.f.style.cssText="max-width:none;display:inline-block;position:absolute;height:100%;width:100%;overflow:scroll;font-size:16px;",this.h.style.cssText="display:inline-block;width:200%;height:200%;font-size:16px;max-width:none;",this.b.appendChild(this.h),this.c.appendChild(this.f),this.a.appendChild(this.b),this.a.appendChild(this.c)}function i(t,e){t.a.style.cssText="max-width:none;min-width:20px;min-height:20px;display:inline-block;overflow:hidden;position:absolute;width:auto;margin:0;padding:0;top:-999px;left:-999px;white-space:nowrap;font:"+e+";"}function o(t){var e=t.a.offsetWidth,n=e+100;return t.f.style.width=n+"px",t.c.scrollLeft=n,t.b.scrollLeft=t.b.scrollWidth+100,t.g!==e?(t.g=e,!0):!1}function r(e,n){function i(){var t=r;o(t)&&null!==t.a.parentNode&&n(t.g)}var r=e;t(e.b,i),t(e.c,i),o(e)}function s(t,e){var n=e||{};this.family=t,this.style=n.style||"normal",this.weight=n.weight||"normal",this.stretch=n.stretch||"normal"}function a(){if(null===d){var t=document.createElement("div");try{t.style.font="condensed 100px sans-serif"}catch(e){}d=""!==t.style.font}return d}function c(t,e){return[t.style,t.weight,a()?t.stretch:"","100px",e].join(" ")}var u=!!document.addEventListener,l=null,f=null,d=null,h=null;s.prototype.load=function(t,o){var s=this,a=t||"BESbswy",u=0,d=o||3e3,m=(new Date).getTime();return new Promise(function(t,o){var p;if(null===h&&(h=!!document.fonts),(p=h)&&(null===f&&(f=/OS X.*Version\/10\..*Safari/.test(navigator.userAgent)&&/Apple/.test(navigator.vendor)),p=!f),p){p=new Promise(function(t,e){function n(){(new Date).getTime()-m>=d?e():document.fonts.load(c(s,'"'+s.family+'"'),a).then(function(e){1<=e.length?t():setTimeout(n,25)},function(){e()})}n()});var v=new Promise(function(t,e){u=setTimeout(e,d)});Promise.race([v,p]).then(function(){clearTimeout(u),t(s)},function(){o(s)})}else e(function(){function e(){var e;(e=-1!=w&&-1!=y||-1!=w&&-1!=g||-1!=y&&-1!=g)&&((e=w!=y&&w!=g&&y!=g)||(null===l&&(e=/AppleWebKit\/([0-9]+)(?:\.([0-9]+))/.exec(window.navigator.userAgent),l=!!e&&(536>parseInt(e[1],10)||536===parseInt(e[1],10)&&11>=parseInt(e[2],10))),e=l&&(w==b&&y==b&&g==b||w==S&&y==S&&g==S||w==C&&y==C&&g==C)),e=!e),e&&(null!==x.parentNode&&x.parentNode.removeChild(x),clearTimeout(u),t(s))}function f(){if((new Date).getTime()-m>=d)null!==x.parentNode&&x.parentNode.removeChild(x),o(s);else{var t=document.hidden;(!0===t||void 0===t)&&(w=h.a.offsetWidth,y=p.a.offsetWidth,g=v.a.offsetWidth,e()),u=setTimeout(f,50)}}var h=new n(a),p=new n(a),v=new n(a),w=-1,y=-1,g=-1,b=-1,S=-1,C=-1,x=document.createElement("div");x.dir="ltr",i(h,c(s,"sans-serif")),i(p,c(s,"serif")),i(v,c(s,"monospace")),x.appendChild(h.a),x.appendChild(p.a),x.appendChild(v.a),document.body.appendChild(x),b=h.a.offsetWidth,S=p.a.offsetWidth,C=v.a.offsetWidth,f(),r(h,function(t){w=t,e()}),i(h,c(s,'"'+s.family+'",sans-serif')),r(p,function(t){y=t,e()}),i(p,c(s,'"'+s.family+'",serif')),r(v,function(t){g=t,e()}),i(v,c(s,'"'+s.family+'",monospace'))})})},"undefined"!=typeof module?module.exports=s:(window.FontFaceObserver=s,window.FontFaceObserver.prototype.load=s.prototype.load)}();var UCASDesignFramework=UCASDesignFramework||{};"visibilityState"in document&&!function(t){function e(){this._queues=[],this._wildcard="*"}e.prototype.subscribe=function(t,e){var n=this._queues.filter(function(e){return e.name===t}).length;return n?this._queues.forEach(function(n){n.name===t&&n.listeners.push(e)}):this._queues.push({name:t,listeners:[e]}),{unsubscribe:function(){this._queues.forEach(function(n){n.name===t&&(n.listeners=n.listeners.filter(function(t){return t!==e}))})}.bind(this),messageType:t,callback:e}},e.prototype.publish=function(t,e){var n=t.split(".");this._queues.filter(function(t){for(var e=t.name.split("."),i=0;i<e.length;i++)if(e[i]!==this._wildcard&&n[i]!==e[i])return!1;return!0}.bind(this)).forEach(function(t){t.listeners.forEach(function(t){t(e)})})},t.bus=new e}(UCASDesignFramework);var UCASDesignFramework=UCASDesignFramework||{};UCASDesignFramework.subscriptions=UCASDesignFramework.subscriptions||{},"visibilityState"in document&&!function(t){var e=t.bus.subscribe("df.layout.wide",function(){document.body.setAttribute("data-fullwidthgrid","")}),n=t.bus.subscribe("df.layout.standard",function(){document.body.removeAttribute("data-fullwidthgrid")}),i=t.bus.subscribe("df.style.version",function(t){document.documentElement.setAttribute("data-version",t)}),o=t.bus.subscribe("df.title.set",function(t){var e="UCAS | ";if(!t){var n=document.querySelector("h1");t=n?n.innerText:"At the heart of connecting people to higher education"}document.title=e+t});t.subscriptions.df={layoutWide:e,layoutStandard:n,title:o,version:i}}(UCASDesignFramework);var UCASUtilities=UCASUtilities||{};!function(t){function e(t,e){"use strict";function i(t){try{var e=window[t],n="__storage_test__";return e.setItem(n,n),e.removeItem(n),!0}catch(i){return!1}}var r=!1;if(i("localStorage")&&(r=!0),"undefined"!=typeof e&&null!==e&&("object"==typeof e&&(e=JSON.stringify(e)),r?localStorage.setItem(t,e):n(t,e,30)),"undefined"==typeof e){var s;s=r?localStorage.getItem(t):o(t);try{s=JSON.parse(s)}catch(a){}return s}null===e&&(r?localStorage.removeItem(t):n(t,"",-1))}function n(t,e,n,o){n=n||30,o=o||i();var r=new Date;r.setTime(r.getTime()+24*n*60*60*1e3);var s="; expires="+r.toGMTString(),a=o?"; domain="+o:null;document.cookie=t+"="+e+s+a+"; path=/; SameSite=None; Secure;"}function i(){var t=window.location.hostname;return/ucas.com$/.test(t)?".ucas.com":/ucasenvironments.com$/.test(t)?".ucasenvironments.com":!1}function o(t){for(var e=t+"=",n=document.cookie.split(";"),i=0,o=n.length;o>i;i++){for(var r=n[i];" "===r.charAt(0);)r=r.substring(1,r.length);if(0===r.indexOf(e))return r.substring(e.length,r.length)}return null}function r(t){return e(t)}function s(t,n){return e(t,n)}function a(t){return e(t,null)}function c(t){return o(t)}function u(t,e,i,o){return n(t,e,i,o)}function l(t,e){return n(t,null,-1,e)}t.store={get:r,set:s,remove:a,getCookie:c,setCookie:u,removeCookie:l}}(UCASUtilities),void 0===Array.prototype.map&&(Array.prototype.map=function(t){for(var e=[],n=0,i=this.length;i>n;n++)e.push(t(this[n]));return e}),function(){if(-1===document.documentElement.className.indexOf("fontsloaded"))if(-1!==document.cookie.indexOf("fontsloaded="))document.documentElement.className+=" fontsloaded";else{var t=["bold","normal",500].map(function(t){return new FontFaceObserver("Roboto",{weight:t})}).map(function(t){return t.load()["catch"](function(t){return t})});Promise.all(t).then(function(){document.documentElement.className+=" fontsloaded",document.cookie="fontsloaded=1; path=/; SameSite=None; Secure;"})["catch"]()}}();var UCASDesignFramework=UCASDesignFramework||{};"visibilityState"in document&&(UCASDesignFramework.promise=new Promise(function(t,e){document.addEventListener("designFrameworkReady",function n(){document.removeEventListener("designFrameworkReady",n),t()})}));