import * as __WEBPACK_EXTERNAL_MODULE_htmx_org_ba298f14__ from "htmx.org";
/******/ var __webpack_modules__ = ({

/***/ "./wwwroot/js/rizzy-nonce.js":
/*!***********************************!*\
  !*** ./wwwroot/js/rizzy-nonce.js ***!
  \***********************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony import */ var htmx__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! htmx */ "htmx");

(function () {
  htmx__WEBPACK_IMPORTED_MODULE_0__["default"].defineExtension('rizzy-nonce', {
    transformResponse: function transformResponse(text, xhr, elt) {
      var _htmx$config$document;
      var documentNonce = (_htmx$config$document = htmx__WEBPACK_IMPORTED_MODULE_0__["default"].config.documentNonce) !== null && _htmx$config$document !== void 0 ? _htmx$config$document : htmx__WEBPACK_IMPORTED_MODULE_0__["default"].config.inlineScriptNonce;
      if (!documentNonce) {
        console.warn("rizzy-nonce extension loaded but no no nonce found for document. Inline scripts may be blocked.");
        documentNonce = "";
      }

      // disable ajax fetching on history miss because it doesn't handle nonce replacment
      htmx__WEBPACK_IMPORTED_MODULE_0__["default"].config.refreshOnHistoryMiss = true;

      // CSP nonce determination based on safe-nonce by Michael West
      var nonce = xhr === null || xhr === void 0 ? void 0 : xhr.getResponseHeader('HX-Nonce');
      if (!nonce) {
        var csp = xhr === null || xhr === void 0 ? void 0 : xhr.getResponseHeader('content-security-policy');
        if (csp) {
          var cspMatch = csp.match(/(style|script)-src[^;]*'nonce-([^']*)'/i);
          if (cspMatch) {
            nonce = cspMatch[2];
          }
        }
      }
      if (xhr && window.location.hostname) {
        var responseURL = new URL(xhr.responseURL);
        if (responseURL.hostname !== window.location.hostname) {
          nonce = ''; // ignore nonce header if request is not some domain 
        }
      }
      nonce !== null && nonce !== void 0 ? nonce : nonce = '';
      return this.processUnsafeHtml(text, documentNonce, nonce);
    },
    processUnsafeHtml: function processUnsafeHtml(text, documentNonce, newScriptNonce) {
      //const noncePattern = new RegExp(`(['"])${newScriptNonce}\\1`, 'gi');

      // Replace any occurrences of the nonce provided by the server with
      // the existing document nonce. Note that at minimum the server text originates
      // from is same-origin and the newScriptNonce that is replaced is determined
      // from response headers which are only available when processing the xmlHttpRequest
      if (documentNonce) text = text.replaceAll(newScriptNonce, documentNonce);
      var parser = new DOMParser();
      try {
        // At this point any remaining elements that don't have the correct
        // nonce will cause console errors to be emitted. We are going to strip
        // out those elements and any attempts to block rizzy-nonce in the included markup.
        var doc = parser.parseFromString(text, "text/html");
        if (doc) {
          // Remove any attempts to disable rizzy-nonce extension
          Array.from(doc.querySelectorAll('[hx-ext*="ignore:rizzy-nonce"], [data-hx-ext*="ignore:rizzy-nonce"]')).forEach(function (elt) {
            elt.remove();
          });

          // Select all <script> and <style> tags
          var elements = doc.querySelectorAll("script, style, link");

          // Iterate through each element
          elements.forEach(function (elt) {
            var nonce = elt.getAttribute("nonce");
            if (nonce !== documentNonce) {
              // Remove the element if the nonce doesn't match (or is missing)
              elt.remove();
            }
          });

          // Serialize the document back into an HTML string and return it
          return doc.documentElement.outerHTML;
        }
      } catch (_) {}
      {}
      return '';
    }
  });
})();

/***/ }),

/***/ "./wwwroot/js/rizzy-streaming.js":
/*!***************************************!*\
  !*** ./wwwroot/js/rizzy-streaming.js ***!
  \***************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony import */ var htmx__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! htmx */ "htmx");
function _typeof(o) { "@babel/helpers - typeof"; return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (o) { return typeof o; } : function (o) { return o && "function" == typeof Symbol && o.constructor === Symbol && o !== Symbol.prototype ? "symbol" : typeof o; }, _typeof(o); }
function _classCallCheck(a, n) { if (!(a instanceof n)) throw new TypeError("Cannot call a class as a function"); }
function _defineProperties(e, r) { for (var t = 0; t < r.length; t++) { var o = r[t]; o.enumerable = o.enumerable || !1, o.configurable = !0, "value" in o && (o.writable = !0), Object.defineProperty(e, _toPropertyKey(o.key), o); } }
function _createClass(e, r, t) { return r && _defineProperties(e.prototype, r), t && _defineProperties(e, t), Object.defineProperty(e, "prototype", { writable: !1 }), e; }
function _toPropertyKey(t) { var i = _toPrimitive(t, "string"); return "symbol" == _typeof(i) ? i : i + ""; }
function _toPrimitive(t, r) { if ("object" != _typeof(t) || !t) return t; var e = t[Symbol.toPrimitive]; if (void 0 !== e) { var i = e.call(t, r || "default"); if ("object" != _typeof(i)) return i; throw new TypeError("@@toPrimitive must return a primitive value."); } return ("string" === r ? String : Number)(t); }
function _callSuper(t, o, e) { return o = _getPrototypeOf(o), _possibleConstructorReturn(t, _isNativeReflectConstruct() ? Reflect.construct(o, e || [], _getPrototypeOf(t).constructor) : o.apply(t, e)); }
function _possibleConstructorReturn(t, e) { if (e && ("object" == _typeof(e) || "function" == typeof e)) return e; if (void 0 !== e) throw new TypeError("Derived constructors may only return object or undefined"); return _assertThisInitialized(t); }
function _assertThisInitialized(e) { if (void 0 === e) throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); return e; }
function _inherits(t, e) { if ("function" != typeof e && null !== e) throw new TypeError("Super expression must either be null or a function"); t.prototype = Object.create(e && e.prototype, { constructor: { value: t, writable: !0, configurable: !0 } }), Object.defineProperty(t, "prototype", { writable: !1 }), e && _setPrototypeOf(t, e); }
function _wrapNativeSuper(t) { var r = "function" == typeof Map ? new Map() : void 0; return _wrapNativeSuper = function _wrapNativeSuper(t) { if (null === t || !_isNativeFunction(t)) return t; if ("function" != typeof t) throw new TypeError("Super expression must either be null or a function"); if (void 0 !== r) { if (r.has(t)) return r.get(t); r.set(t, Wrapper); } function Wrapper() { return _construct(t, arguments, _getPrototypeOf(this).constructor); } return Wrapper.prototype = Object.create(t.prototype, { constructor: { value: Wrapper, enumerable: !1, writable: !0, configurable: !0 } }), _setPrototypeOf(Wrapper, t); }, _wrapNativeSuper(t); }
function _construct(t, e, r) { if (_isNativeReflectConstruct()) return Reflect.construct.apply(null, arguments); var o = [null]; o.push.apply(o, e); var p = new (t.bind.apply(t, o))(); return r && _setPrototypeOf(p, r.prototype), p; }
function _isNativeReflectConstruct() { try { var t = !Boolean.prototype.valueOf.call(Reflect.construct(Boolean, [], function () {})); } catch (t) {} return (_isNativeReflectConstruct = function _isNativeReflectConstruct() { return !!t; })(); }
function _isNativeFunction(t) { try { return -1 !== Function.toString.call(t).indexOf("[native code]"); } catch (n) { return "function" == typeof t; } }
function _setPrototypeOf(t, e) { return _setPrototypeOf = Object.setPrototypeOf ? Object.setPrototypeOf.bind() : function (t, e) { return t.__proto__ = e, t; }, _setPrototypeOf(t, e); }
function _getPrototypeOf(t) { return _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf.bind() : function (t) { return t.__proto__ || Object.getPrototypeOf(t); }, _getPrototypeOf(t); }
/*
 * Blazor Stream Rendering HTMX Extension
 * Author: Michael Tanczos
 * Credits to SSE extension and Microsoft aspnetcore
 * at https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web.JS/src/Rendering/StreamingRendering.ts
 */

(function () {
  var api;
  var enableDomPreservation = true;
  var componentLoaded = false;
  var blazorStreamingUpdate = /*#__PURE__*/function (_HTMLElement) {
    function blazorStreamingUpdate() {
      _classCallCheck(this, blazorStreamingUpdate);
      return _callSuper(this, blazorStreamingUpdate, arguments);
    }
    _inherits(blazorStreamingUpdate, _HTMLElement);
    return _createClass(blazorStreamingUpdate, [{
      key: "connectedCallback",
      value: function connectedCallback() {
        var _blazorSsrElement$par;
        var blazorSsrElement = this.parentNode;

        // Synchronously remove this from the DOM to minimize our chance of affecting anything else
        (_blazorSsrElement$par = blazorSsrElement.parentNode) === null || _blazorSsrElement$par === void 0 || _blazorSsrElement$par.removeChild(blazorSsrElement);

        // When this element receives content, if it's <template blazor-component-id="...">...</template>,
        // insert the template content into the DOM
        blazorSsrElement.childNodes.forEach(function (node) {
          if (node instanceof HTMLTemplateElement) {
            var componentId = node.getAttribute("blazor-component-id");
            if (componentId) {
              insertStreamingContentIntoDocument(componentId, node.content);
            }
          }
        });
        htmx__WEBPACK_IMPORTED_MODULE_0__["default"] === null || htmx__WEBPACK_IMPORTED_MODULE_0__["default"] === void 0 || htmx__WEBPACK_IMPORTED_MODULE_0__["default"].process(document.body);
      }
    }]);
  }(/*#__PURE__*/_wrapNativeSuper(HTMLElement));
  htmx__WEBPACK_IMPORTED_MODULE_0__["default"].defineExtension("rizzy-streaming", {
    /**
     * Init saves the provided reference to the internal HTMX API.
     *
     * @param {import("../htmx").HtmxInternalApi} api
     * @returns void
     */
    init: function init(apiRef) {
      // store a reference to the internal API.
      api = apiRef;

      // set a function in the public API for creating new EventSource objects
      if (htmx__WEBPACK_IMPORTED_MODULE_0__["default"].blazorSwapSsr == undefined) {
        if (customElements.get('blazor-ssr-end') === undefined) {
          customElements.define('blazor-ssr-end', blazorStreamingUpdate);
        }
        htmx__WEBPACK_IMPORTED_MODULE_0__["default"].blazorSwapSsr = blazorSwapSsr;
      }
    },
    onEvent: function onEvent(name, evt) {
      if (name === "htmx:afterOnLoad") {
        htmx__WEBPACK_IMPORTED_MODULE_0__["default"] === null || htmx__WEBPACK_IMPORTED_MODULE_0__["default"] === void 0 || htmx__WEBPACK_IMPORTED_MODULE_0__["default"].process(document.body);
      } else if (name === "htmx:beforeRequest") {
        var element = evt.detail.elt;
        if (evt.detail.requestConfig.target) {
          evt.detail.requestConfig.target.addEventListener("htmx:beforeSwap", function (e) {
            // Any html that was already streamed in could have been updated with
            // blazor ssr content so the final xhr response can be thrown away
            //e.detail.shouldSwap = false;
          }, {
            once: true
          });
        }
        var last = 0;
        var swapSpec = api.getSwapSpecification(element);
        var xhr = evt.detail.xhr;

        // Create a container id for a temporary div container. All streamed html will be placed 
        // inside the container so that htmx swap methods work correctly
        var cid = 'ctr' + crypto.randomUUID();
        xhr.addEventListener("readystatechange", function () {
          // If finished we can unwrap the container all html was stored into
          if (xhr.readyState === 4) {
            var container = document.getElementById(cid);
            if (container != null) unwrap(container);
          }
        });
        xhr.addEventListener("progress", function (e) {
          var container = document.getElementById(cid);

          // If the container doesn't exist we need to create it and swap it into the element
          // target space. From here on we can stream responses into the container directly.
          if (container == null) {
            var _document$getElementB;
            container = document.createElement('div');
            container.id = cid;

            // Swap in a container div to hold the streaming html
            swap(element, container.outerHTML, swapSpec, xhr);

            // The very first swap into the container can be a replacement swap
            swapSpec.swapStyle = "innerHTML";

            // Ensure there is always a container even if not added to the dom
            container = (_document$getElementB = document.getElementById(cid)) !== null && _document$getElementB !== void 0 ? _document$getElementB : container;
          }

          // Compute any new html in this chunk
          diff = e.currentTarget.response.substring(last);
          swap(container, diff, swapSpec, xhr);
          swapSpec.settleDelay = 0;
          swapSpec.swapStyle = "beforeend";
          last = e.loaded;
        });
      }
      return true;
    }
  });
  function isCommentNodeInHead(commentNode) {
    // Ensure that the provided node is indeed a comment node
    if (commentNode && commentNode.nodeType === Node.COMMENT_NODE) {
      var currentNode = commentNode.parentNode;
      // Traverse up the DOM tree
      while (currentNode !== null) {
        if (currentNode === document.head) {
          // The comment node is within the <head>
          return true;
        }
        currentNode = currentNode.parentNode;
      }
    } else {
      return false;
    }
    // The traversal reached the root without finding <head>, or <head> does not exist
    return false;
  }
  function blazorSwapSsr(start, end, docFrag, xhr) {
    var newDiv = wrap(start, end, 'ssr' + crypto.randomUUID());
    var container = document.createElement('div');
    container.appendChild(docFrag);
    swap(newDiv, container.innerHTML, xhr);
    unwrap(newDiv);
  }
  function wrap(start, end, id) {
    var newDiv = document.createElement('div');
    newDiv.id = id;

    // Iterate through nodes between start and end
    var currentNode = start.nextSibling;
    while (currentNode && currentNode !== end) {
      newDiv.appendChild(currentNode);
      currentNode = start.nextSibling;
    }
    start.parentNode.insertBefore(newDiv, end);
    return newDiv;
  }
  function unwrap(element) {
    // Ensure that the element has a parent
    if (element.parentNode) {
      // Move all child nodes out of the element
      while (element.firstChild) {
        element.parentNode.insertBefore(element.firstChild, element);
      }

      // Remove the empty element
      element.parentNode.removeChild(element);
    }
  }
  function handleOutOfBandSwaps(elt, fragment, settleInfo) {
    var oobSelects = api.getClosestAttributeValue(elt, "hx-select-oob");
    if (oobSelects) {
      var oobSelectValues = oobSelects.split(",");
      for (var i = 0; i < oobSelectValues.length; i++) {
        var oobSelectValue = oobSelectValues[i].split(":", 2);
        var id = oobSelectValue[0].trim();
        if (id.indexOf("#") === 0) {
          id = id.substring(1);
        }
        var oobValue = oobSelectValue[1] || "true";
        var oobElement = fragment.querySelector("#" + id);
        if (oobElement) {
          api.oobSwap(oobValue, oobElement, settleInfo);
        }
      }
    }
    forEach(findAll(fragment, '[hx-swap-oob], [data-hx-swap-oob]'), function (oobElement) {
      var oobValue = getAttributeValue(oobElement, "hx-swap-oob");
      if (oobValue != null) {
        api.oobSwap(oobValue, oobElement, settleInfo);
      }
    });
  }

  /**
   * @param {HTMLElement} elt
   * @param {string} content
   */
  function swap(elt, content, swapSpec, xhr) {
    api.withExtensions(elt, function (extension) {
      content = extension.transformResponse(content, xhr, elt);
    });
    swapSpec !== null && swapSpec !== void 0 ? swapSpec : swapSpec = api.getSwapSpecification(elt);
    var target = api.getTarget(elt);
    var settleInfo = api.makeSettleInfo(elt);

    // htmx 2.0
    api.swap(target, content, swapSpec);

    //api.selectAndSwap(swapSpec.swapStyle, target, elt, content, settleInfo);

    settleInfo.elts.forEach(function (elt) {
      if (elt.classList) {
        elt.classList.add(htmx__WEBPACK_IMPORTED_MODULE_0__["default"].config.settlingClass);
      }
      api.triggerEvent(elt, 'htmx:beforeSettle');
    });

    // Handle settle tasks (with delay if requested)
    if (swapSpec.settleDelay > 0) {
      setTimeout(doSettle(settleInfo), swapSpec.settleDelay);
    } else {
      doSettle(settleInfo)();
    }
  }

  /**
   * doSettle mirrors much of the functionality in htmx that
   * settles elements after their content has been swapped.
   * TODO: this should be published by htmx, and not duplicated here
   * @param {import("../htmx").HtmxSettleInfo} settleInfo
   * @returns () => void
   */
  function doSettle(settleInfo) {
    return function () {
      settleInfo.tasks.forEach(function (task) {
        task.call();
      });
      settleInfo.elts.forEach(function (elt) {
        if (elt.classList) {
          elt.classList.remove(htmx__WEBPACK_IMPORTED_MODULE_0__["default"].config.settlingClass);
        }
        api.triggerEvent(elt, 'htmx:afterSettle');
      });
    };
  }
  function insertStreamingContentIntoDocument(componentIdAsString, docFrag) {
    var markers = findStreamingMarkers(componentIdAsString);
    if (markers) {
      var startMarker = markers.startMarker,
        endMarker = markers.endMarker;
      enableDomPreservation = !isCommentNodeInHead(startMarker);
      if (enableDomPreservation) {
        blazorSwapSsr(startMarker, endMarker, docFrag);
      } else {
        // In this mode we completely delete the old content before inserting the new content
        var destinationRoot = endMarker.parentNode;
        var existingContent = new Range();
        existingContent.setStart(startMarker, startMarker.textContent.length);
        existingContent.setEnd(endMarker, 0);
        existingContent.deleteContents();
        while (docFrag.childNodes[0]) {
          destinationRoot.insertBefore(docFrag.childNodes[0], endMarker);
        }
      }
    }
  }
  function findStreamingMarkers(componentIdAsString) {
    // Find start marker
    var expectedStartText = "bl:".concat(componentIdAsString);
    var iterator = document.createNodeIterator(document, NodeFilter.SHOW_COMMENT);
    var startMarker = null;
    while (startMarker = iterator.nextNode()) {
      if (startMarker.textContent === expectedStartText) {
        break;
      }
    }
    if (!startMarker) {
      return null;
    }

    // Find end marker
    var expectedEndText = "/bl:".concat(componentIdAsString);
    var endMarker = null;
    while (endMarker = iterator.nextNode()) {
      if (endMarker.textContent === expectedEndText) {
        break;
      }
    }
    return endMarker ? {
      startMarker: startMarker,
      endMarker: endMarker
    } : null;
  }
})();

/***/ }),

/***/ "htmx":
/*!***************************!*\
  !*** external "htmx.org" ***!
  \***************************/
/***/ ((module) => {

module.exports = __WEBPACK_EXTERNAL_MODULE_htmx_org_ba298f14__;

/***/ })

/******/ });
/************************************************************************/
/******/ // The module cache
/******/ var __webpack_module_cache__ = {};
/******/ 
/******/ // The require function
/******/ function __webpack_require__(moduleId) {
/******/ 	// Check if module is in cache
/******/ 	var cachedModule = __webpack_module_cache__[moduleId];
/******/ 	if (cachedModule !== undefined) {
/******/ 		return cachedModule.exports;
/******/ 	}
/******/ 	// Create a new module (and put it into the cache)
/******/ 	var module = __webpack_module_cache__[moduleId] = {
/******/ 		// no module.id needed
/******/ 		// no module.loaded needed
/******/ 		exports: {}
/******/ 	};
/******/ 
/******/ 	// Execute the module function
/******/ 	__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 
/******/ 	// Return the exports of the module
/******/ 	return module.exports;
/******/ }
/******/ 
/************************************************************************/
/******/ /* webpack/runtime/make namespace object */
/******/ (() => {
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = (exports) => {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/ })();
/******/ 
/************************************************************************/
var __webpack_exports__ = {};
// This entry needs to be wrapped in an IIFE because it needs to be isolated against other modules in the chunk.
(() => {
/*!*****************************!*\
  !*** ./wwwroot/js/rizzy.js ***!
  \*****************************/
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _rizzy_nonce__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./rizzy-nonce */ "./wwwroot/js/rizzy-nonce.js");
/* harmony import */ var _rizzy_streaming__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./rizzy-streaming */ "./wwwroot/js/rizzy-streaming.js");


})();


//# sourceMappingURL=rizzy.js.map