(function(global, factory) {
  typeof exports === "object" && typeof module !== "undefined" ? module.exports = factory() : typeof define === "function" && define.amd ? define(factory) : (global = typeof globalThis !== "undefined" ? globalThis : global || self, global.Rizzy = factory());
})(this, (function() {
  "use strict";
  (function() {
    htmx.defineExtension(
      "rizzy-nonce",
      {
        transformResponse: function(text, xhr, elt) {
          let documentNonce = htmx.config.documentNonce ?? htmx.config.inlineScriptNonce;
          if (!documentNonce) {
            console.warn("rizzy-nonce extension loaded but no no nonce found for document. Inline scripts may be blocked.");
            documentNonce = "";
          }
          htmx.config.refreshOnHistoryMiss = true;
          let nonce = xhr?.getResponseHeader("HX-Nonce");
          if (!nonce) {
            const csp = xhr?.getResponseHeader("content-security-policy");
            if (csp) {
              const cspMatch = csp.match(/(style|script)-src[^;]*'nonce-([^']*)'/i);
              if (cspMatch) {
                nonce = cspMatch[2];
              }
            }
          }
          if (xhr && window.location.hostname) {
            const responseURL = new URL(xhr.responseURL);
            if (responseURL.hostname !== window.location.hostname) {
              nonce = "";
            }
          }
          nonce ?? (nonce = "");
          return this.processUnsafeHtml(text, documentNonce, nonce);
        },
        processUnsafeHtml: function(text, documentNonce, newScriptNonce) {
          if (documentNonce && newScriptNonce)
            text = text.replaceAll(newScriptNonce, documentNonce);
          const parser = new DOMParser();
          try {
            let doc = parser.parseFromString(text, "text/html");
            if (doc) {
              Array.from(doc.querySelectorAll('[hx-ext*="ignore:rizzy-nonce"], [data-hx-ext*="ignore:rizzy-nonce"]')).forEach((elt) => {
                elt.remove();
              });
              const elements = doc.querySelectorAll("script, style, link");
              elements.forEach((elt) => {
                const nonce = elt.getAttribute("nonce");
                if (nonce !== documentNonce) {
                  elt.remove();
                }
              });
              return doc.documentElement.outerHTML;
            }
          } catch (_) {
          }
          return "";
        }
      }
    );
  })();
  (function() {
    var api;
    var enableDomPreservation = true;
    class blazorStreamingUpdate extends HTMLElement {
      connectedCallback() {
        const blazorSsrElement = this.parentNode;
        blazorSsrElement.parentNode?.removeChild(blazorSsrElement);
        blazorSsrElement.childNodes.forEach((node) => {
          if (node instanceof HTMLTemplateElement) {
            const componentId = node.getAttribute("blazor-component-id");
            if (componentId) {
              insertStreamingContentIntoDocument(componentId, node.content);
            }
          }
        });
        htmx?.process(document.body);
      }
    }
    htmx.defineExtension(
      "rizzy-streaming",
      {
        /**
         * Init saves the provided reference to the internal HTMX API.
         *
         * @param {import("../htmx").HtmxInternalApi} api
         * @returns void
         */
        init: function(apiRef) {
          api = apiRef;
          if (htmx.blazorSwapSsr == void 0) {
            if (customElements.get("blazor-ssr-end") === void 0) {
              customElements.define("blazor-ssr-end", blazorStreamingUpdate);
            }
            htmx.blazorSwapSsr = blazorSwapSsr;
          }
        },
        onEvent: function(name2, evt) {
          if (name2 === "htmx:afterOnLoad") {
            htmx?.process(document.body);
          } else if (name2 === "htmx:beforeRequest") {
            var element = evt.detail.elt;
            if (evt.detail.requestConfig.target) {
              evt.detail.requestConfig.target.addEventListener(
                "htmx:beforeSwap",
                (e) => {
                },
                { once: true }
              );
            }
            var last = 0;
            var swapSpec = api.getSwapSpecification(element);
            var xhr = evt.detail.xhr;
            var cid = "ctr" + crypto.randomUUID();
            xhr.addEventListener("readystatechange", () => {
              if (xhr.readyState === 4) {
                var container = document.getElementById(cid);
                if (container != null)
                  unwrap(container);
              }
            });
            xhr.addEventListener("progress", (e) => {
              var container = document.getElementById(cid);
              if (container == null) {
                container = document.createElement("div");
                container.id = cid;
                swap(element, container.outerHTML, swapSpec, xhr);
                swapSpec.swapStyle = "innerHTML";
                container = document.getElementById(cid) ?? container;
              }
              let diff = e.currentTarget.response.substring(last);
              swap(container, diff, swapSpec, xhr);
              swapSpec.settleDelay = 0;
              swapSpec.swapStyle = "beforeend";
              last = e.loaded;
            });
          }
          return true;
        }
      }
    );
    function isCommentNodeInHead(commentNode) {
      if (commentNode && commentNode.nodeType === Node.COMMENT_NODE) {
        let currentNode = commentNode.parentNode;
        while (currentNode !== null) {
          if (currentNode === document.head) {
            return true;
          }
          currentNode = currentNode.parentNode;
        }
      } else {
        return false;
      }
      return false;
    }
    function blazorSwapSsr(start, end, docFrag, xhr) {
      var newDiv = wrap(start, end, "ssr" + crypto.randomUUID());
      var container = document.createElement("div");
      container.appendChild(docFrag);
      swap(newDiv, container.innerHTML, xhr);
      unwrap(newDiv);
    }
    function wrap(start, end, id) {
      var newDiv = document.createElement("div");
      newDiv.id = id;
      var currentNode = start.nextSibling;
      while (currentNode && currentNode !== end) {
        newDiv.appendChild(currentNode);
        currentNode = start.nextSibling;
      }
      start.parentNode.insertBefore(newDiv, end);
      return newDiv;
    }
    function unwrap(element) {
      if (element.parentNode) {
        while (element.firstChild) {
          element.parentNode.insertBefore(element.firstChild, element);
        }
        element.parentNode.removeChild(element);
      }
    }
    function swap(elt, content, swapSpec, xhr) {
      api.withExtensions(elt, function(extension) {
        content = extension.transformResponse(content, xhr, elt);
      });
      swapSpec ?? (swapSpec = api.getSwapSpecification(elt));
      var target = api.getTarget(elt);
      var settleInfo = api.makeSettleInfo(elt);
      api.swap(target, content, swapSpec);
      settleInfo.elts.forEach(function(elt2) {
        if (elt2.classList) {
          elt2.classList.add(htmx.config.settlingClass);
        }
        api.triggerEvent(elt2, "htmx:beforeSettle");
      });
      if (swapSpec.settleDelay > 0) {
        setTimeout(doSettle(settleInfo), swapSpec.settleDelay);
      } else {
        doSettle(settleInfo)();
      }
    }
    function doSettle(settleInfo) {
      return function() {
        settleInfo.tasks.forEach(function(task) {
          task.call();
        });
        settleInfo.elts.forEach(function(elt) {
          if (elt.classList) {
            elt.classList.remove(htmx.config.settlingClass);
          }
          api.triggerEvent(elt, "htmx:afterSettle");
        });
      };
    }
    function insertStreamingContentIntoDocument(componentIdAsString, docFrag) {
      const markers = findStreamingMarkers(componentIdAsString);
      if (markers) {
        const { startMarker, endMarker } = markers;
        enableDomPreservation = !isCommentNodeInHead(startMarker);
        if (enableDomPreservation) {
          blazorSwapSsr(startMarker, endMarker, docFrag);
        } else {
          const destinationRoot = endMarker.parentNode;
          const existingContent = new Range();
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
      const expectedStartText = `bl:${componentIdAsString}`;
      const iterator = document.createNodeIterator(
        document,
        NodeFilter.SHOW_COMMENT
      );
      let startMarker = null;
      while (startMarker = iterator.nextNode()) {
        if (startMarker.textContent === expectedStartText) {
          break;
        }
      }
      if (!startMarker) {
        return null;
      }
      const expectedEndText = `/bl:${componentIdAsString}`;
      let endMarker = null;
      while (endMarker = iterator.nextNode()) {
        if (endMarker.textContent === expectedEndText) {
          break;
        }
      }
      return endMarker ? { startMarker, endMarker } : null;
    }
  })();
  var aspnetValidation = { exports: {} };
  var hasRequiredAspnetValidation;
  function requireAspnetValidation() {
    if (hasRequiredAspnetValidation) return aspnetValidation.exports;
    hasRequiredAspnetValidation = 1;
    (function(module2, exports2) {
      (function webpackUniversalModuleDefinition(root, factory) {
        module2.exports = factory();
      })(self, () => {
        return (
          /******/
          (() => {
            var __webpack_require__ = {};
            (() => {
              __webpack_require__.d = (exports3, definition) => {
                for (var key in definition) {
                  if (__webpack_require__.o(definition, key) && !__webpack_require__.o(exports3, key)) {
                    Object.defineProperty(exports3, key, { enumerable: true, get: definition[key] });
                  }
                }
              };
            })();
            (() => {
              __webpack_require__.o = (obj, prop) => Object.prototype.hasOwnProperty.call(obj, prop);
            })();
            (() => {
              __webpack_require__.r = (exports3) => {
                if (typeof Symbol !== "undefined" && Symbol.toStringTag) {
                  Object.defineProperty(exports3, Symbol.toStringTag, { value: "Module" });
                }
                Object.defineProperty(exports3, "__esModule", { value: true });
              };
            })();
            var __webpack_exports__ = {};
            /*!**********************!*\
              !*** ./src/index.ts ***!
              \**********************/
            __webpack_require__.r(__webpack_exports__);
            __webpack_require__.d(__webpack_exports__, {
              /* harmony export */
              MvcValidationProviders: () => (
                /* binding */
                MvcValidationProviders
              ),
              /* harmony export */
              ValidationService: () => (
                /* binding */
                ValidationService
              ),
              /* harmony export */
              isValidatable: () => (
                /* binding */
                isValidatable
              )
              /* harmony export */
            });
            var __awaiter = function(thisArg, _arguments, P, generator) {
              function adopt(value) {
                return value instanceof P ? value : new P(function(resolve) {
                  resolve(value);
                });
              }
              return new (P || (P = Promise))(function(resolve, reject) {
                function fulfilled(value) {
                  try {
                    step(generator.next(value));
                  } catch (e) {
                    reject(e);
                  }
                }
                function rejected(value) {
                  try {
                    step(generator["throw"](value));
                  } catch (e) {
                    reject(e);
                  }
                }
                function step(result) {
                  result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected);
                }
                step((generator = generator.apply(thisArg, _arguments || [])).next());
              });
            };
            var __generator = function(thisArg, body) {
              var _ = { label: 0, sent: function() {
                if (t[0] & 1) throw t[1];
                return t[1];
              }, trys: [], ops: [] }, f, y, t, g;
              return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() {
                return this;
              }), g;
              function verb(n) {
                return function(v) {
                  return step([n, v]);
                };
              }
              function step(op) {
                if (f) throw new TypeError("Generator is already executing.");
                while (g && (g = 0, op[0] && (_ = 0)), _) try {
                  if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
                  if (y = 0, t) op = [op[0] & 2, t.value];
                  switch (op[0]) {
                    case 0:
                    case 1:
                      t = op;
                      break;
                    case 4:
                      _.label++;
                      return { value: op[1], done: false };
                    case 5:
                      _.label++;
                      y = op[1];
                      op = [0];
                      continue;
                    case 7:
                      op = _.ops.pop();
                      _.trys.pop();
                      continue;
                    default:
                      if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) {
                        _ = 0;
                        continue;
                      }
                      if (op[0] === 3 && (!t || op[1] > t[0] && op[1] < t[3])) {
                        _.label = op[1];
                        break;
                      }
                      if (op[0] === 6 && _.label < t[1]) {
                        _.label = t[1];
                        t = op;
                        break;
                      }
                      if (t && _.label < t[2]) {
                        _.label = t[2];
                        _.ops.push(op);
                        break;
                      }
                      if (t[2]) _.ops.pop();
                      _.trys.pop();
                      continue;
                  }
                  op = body.call(thisArg, _);
                } catch (e) {
                  op = [6, e];
                  y = 0;
                } finally {
                  f = t = 0;
                }
                if (op[0] & 5) throw op[1];
                return { value: op[0] ? op[1] : void 0, done: true };
              }
            };
            var nullLogger = new /** @class */
            ((function() {
              function class_1() {
                this.warn = globalThis.console.warn;
              }
              class_1.prototype.log = function(_) {
              };
              return class_1;
            })())();
            var isValidatable = function(element) {
              return element instanceof HTMLInputElement || element instanceof HTMLSelectElement || element instanceof HTMLTextAreaElement;
            };
            var validatableElementTypes = ["input", "select", "textarea"];
            var validatableSelector = function(selector) {
              return validatableElementTypes.map(function(t) {
                return "".concat(t).concat(selector || "");
              }).join(",");
            };
            function getRelativeFormElement(element, selector) {
              var elementName = element.name;
              var selectedName = selector.substring(2);
              var objectName = "";
              var dotLocation = elementName.lastIndexOf(".");
              if (dotLocation > -1) {
                objectName = elementName.substring(0, dotLocation);
                var relativeElementName = objectName + "." + selectedName;
                var relativeElement = document.getElementsByName(relativeElementName)[0];
                if (isValidatable(relativeElement)) {
                  return relativeElement;
                }
              }
              return element.form.querySelector(validatableSelector("[name=".concat(selectedName, "]")));
            }
            var MvcValidationProviders = (
              /** @class */
              /* @__PURE__ */ (function() {
                function MvcValidationProviders2() {
                  this.required = function(value, element, params) {
                    var elementType = element.type.toLowerCase();
                    if (elementType === "checkbox" || elementType === "radio") {
                      var allElementsOfThisName = Array.from(element.form.querySelectorAll(validatableSelector("[name='".concat(element.name, "'][type='").concat(elementType, "']"))));
                      for (var _i = 0, allElementsOfThisName_1 = allElementsOfThisName; _i < allElementsOfThisName_1.length; _i++) {
                        var element_1 = allElementsOfThisName_1[_i];
                        if (element_1 instanceof HTMLInputElement && element_1.checked === true) {
                          return true;
                        }
                      }
                      if (elementType === "checkbox") {
                        var checkboxHiddenInput = element.form.querySelector("input[name='".concat(element.name, "'][type='hidden']"));
                        if (checkboxHiddenInput instanceof HTMLInputElement && checkboxHiddenInput.value === "false") {
                          return true;
                        }
                      }
                      return false;
                    }
                    return Boolean(value === null || value === void 0 ? void 0 : value.trim());
                  };
                  this.stringLength = function(value, element, params) {
                    if (!value) {
                      return true;
                    }
                    if (params.min) {
                      var min = parseInt(params.min);
                      if (value.length < min) {
                        return false;
                      }
                    }
                    if (params.max) {
                      var max = parseInt(params.max);
                      if (value.length > max) {
                        return false;
                      }
                    }
                    return true;
                  };
                  this.compare = function(value, element, params) {
                    if (!params.other) {
                      return true;
                    }
                    var otherElement = getRelativeFormElement(element, params.other);
                    if (!otherElement) {
                      return true;
                    }
                    return otherElement.value === value;
                  };
                  this.range = function(value, element, params) {
                    if (!value) {
                      return true;
                    }
                    var val = parseFloat(value);
                    if (isNaN(val)) {
                      return false;
                    }
                    if (params.min) {
                      var min = parseFloat(params.min);
                      if (val < min) {
                        return false;
                      }
                    }
                    if (params.max) {
                      var max = parseFloat(params.max);
                      if (val > max) {
                        return false;
                      }
                    }
                    return true;
                  };
                  this.regex = function(value, element, params) {
                    if (!value || !params.pattern) {
                      return true;
                    }
                    var r = new RegExp(params.pattern);
                    return r.test(value);
                  };
                  this.email = function(value, element, params) {
                    if (!value) {
                      return true;
                    }
                    var r = /^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*(\.\w{2,})+$/;
                    return r.test(value);
                  };
                  this.creditcard = function(value, element, params) {
                    if (!value) {
                      return true;
                    }
                    if (/[^0-9 \-]+/.test(value)) {
                      return false;
                    }
                    var nCheck = 0, nDigit = 0, bEven = false, n, cDigit;
                    value = value.replace(/\D/g, "");
                    if (value.length < 13 || value.length > 19) {
                      return false;
                    }
                    for (n = value.length - 1; n >= 0; n--) {
                      cDigit = value.charAt(n);
                      nDigit = parseInt(cDigit, 10);
                      if (bEven) {
                        if ((nDigit *= 2) > 9) {
                          nDigit -= 9;
                        }
                      }
                      nCheck += nDigit;
                      bEven = !bEven;
                    }
                    return nCheck % 10 === 0;
                  };
                  this.url = function(value, element, params) {
                    if (!value) {
                      return true;
                    }
                    var lowerCaseValue = value.toLowerCase();
                    return lowerCaseValue.indexOf("http://") > -1 || lowerCaseValue.indexOf("https://") > -1 || lowerCaseValue.indexOf("ftp://") > -1;
                  };
                  this.phone = function(value, element, params) {
                    if (!value) {
                      return true;
                    }
                    var consecutiveSeparator = /[\+\-\s][\-\s]/g;
                    if (consecutiveSeparator.test(value)) {
                      return false;
                    }
                    var r = /^\+?[0-9\-\s]+$/;
                    return r.test(value);
                  };
                  this.remote = function(value, element, params) {
                    if (!value) {
                      return true;
                    }
                    var fieldSelectors = params.additionalfields.split(",");
                    var fields = {};
                    for (var _i = 0, fieldSelectors_1 = fieldSelectors; _i < fieldSelectors_1.length; _i++) {
                      var fieldSelector = fieldSelectors_1[_i];
                      var fieldName = fieldSelector.substr(2);
                      var fieldElement = getRelativeFormElement(element, fieldSelector);
                      var hasValue = Boolean(fieldElement && fieldElement.value);
                      if (!hasValue) {
                        continue;
                      }
                      if (fieldElement instanceof HTMLInputElement && (fieldElement.type === "checkbox" || fieldElement.type === "radio")) {
                        fields[fieldName] = fieldElement.checked ? fieldElement.value : "";
                      } else {
                        fields[fieldName] = fieldElement.value;
                      }
                    }
                    var url = params["url"];
                    var encodedParams = [];
                    for (var fieldName in fields) {
                      var encodedParam = encodeURIComponent(fieldName) + "=" + encodeURIComponent(fields[fieldName]);
                      encodedParams.push(encodedParam);
                    }
                    var payload = encodedParams.join("&");
                    return new Promise(function(ok, reject) {
                      var request = new XMLHttpRequest();
                      if (params.type && params.type.toLowerCase() === "post") {
                        var postData = new FormData();
                        for (var fieldName2 in fields) {
                          postData.append(fieldName2, fields[fieldName2]);
                        }
                        request.open("post", url);
                        request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.send(payload);
                      } else {
                        request.open("get", url + "?" + payload);
                        request.send();
                      }
                      request.onload = function(e) {
                        if (request.status >= 200 && request.status < 300) {
                          var data = JSON.parse(request.responseText);
                          ok(data);
                        } else {
                          reject({
                            status: request.status,
                            statusText: request.statusText,
                            data: request.responseText
                          });
                        }
                      };
                      request.onerror = function(e) {
                        reject({
                          status: request.status,
                          statusText: request.statusText,
                          data: request.responseText
                        });
                      };
                    });
                  };
                }
                return MvcValidationProviders2;
              })()
            );
            var ValidationService = (
              /** @class */
              (function() {
                function ValidationService2(logger) {
                  var _this = this;
                  this.providers = {};
                  this.messageFor = {};
                  this.elementUIDs = [];
                  this.elementByUID = {};
                  this.formInputs = {};
                  this.validators = {};
                  this.formEvents = {};
                  this.inputEvents = {};
                  this.summary = {};
                  this.debounce = 300;
                  this.allowHiddenFields = false;
                  this.validateForm = function(form, callback) {
                    return __awaiter(_this, void 0, void 0, function() {
                      var formUID, formValidationEvent, _a;
                      return __generator(this, function(_b) {
                        switch (_b.label) {
                          case 0:
                            if (!(form instanceof HTMLFormElement)) {
                              throw new Error("validateForm() can only be called on <form> elements");
                            }
                            formUID = this.getElementUID(form);
                            formValidationEvent = this.formEvents[formUID];
                            _a = !formValidationEvent;
                            if (_a) return [3, 2];
                            return [4, formValidationEvent(void 0, callback)];
                          case 1:
                            _a = _b.sent();
                            _b.label = 2;
                          case 2:
                            return [2, _a];
                        }
                      });
                    });
                  };
                  this.validateField = function(field, callback) {
                    return __awaiter(_this, void 0, void 0, function() {
                      var fieldUID, fieldValidationEvent, _a;
                      return __generator(this, function(_b) {
                        switch (_b.label) {
                          case 0:
                            fieldUID = this.getElementUID(field);
                            fieldValidationEvent = this.inputEvents[fieldUID];
                            _a = !fieldValidationEvent;
                            if (_a) return [3, 2];
                            return [4, fieldValidationEvent(void 0, callback)];
                          case 1:
                            _a = _b.sent();
                            _b.label = 2;
                          case 2:
                            return [2, _a];
                        }
                      });
                    });
                  };
                  this.preValidate = function(submitEvent) {
                    submitEvent.preventDefault();
                    submitEvent.stopImmediatePropagation();
                  };
                  this.handleValidated = function(form, success, submitEvent) {
                    if (!(form instanceof HTMLFormElement)) {
                      throw new Error("handleValidated() can only be called on <form> elements");
                    }
                    if (success) {
                      if (submitEvent) {
                        _this.submitValidForm(form, submitEvent);
                      }
                    } else {
                      _this.focusFirstInvalid(form);
                    }
                  };
                  this.submitValidForm = function(form, submitEvent) {
                    if (!(form instanceof HTMLFormElement)) {
                      throw new Error("submitValidForm() can only be called on <form> elements");
                    }
                    var newEvent = new SubmitEvent("submit", submitEvent);
                    if (form.dispatchEvent(newEvent)) {
                      var submitter = submitEvent.submitter;
                      var submitterInput = null;
                      var initialFormAction = form.action;
                      if (submitter) {
                        var name_1 = submitter.getAttribute("name");
                        if (name_1) {
                          submitterInput = document.createElement("input");
                          submitterInput.type = "hidden";
                          submitterInput.name = name_1;
                          submitterInput.value = submitter.getAttribute("value");
                          form.appendChild(submitterInput);
                        }
                        var formAction = submitter.getAttribute("formaction");
                        if (formAction) {
                          form.action = formAction;
                        }
                      }
                      try {
                        form.submit();
                      } finally {
                        if (submitterInput) {
                          form.removeChild(submitterInput);
                        }
                        form.action = initialFormAction;
                      }
                    }
                  };
                  this.focusFirstInvalid = function(form) {
                    if (!(form instanceof HTMLFormElement)) {
                      throw new Error("focusFirstInvalid() can only be called on <form> elements");
                    }
                    var formUID = _this.getElementUID(form);
                    var formInputUIDs = _this.formInputs[formUID];
                    var invalidFormInputUID = formInputUIDs === null || formInputUIDs === void 0 ? void 0 : formInputUIDs.find(function(uid) {
                      return _this.summary[uid];
                    });
                    if (invalidFormInputUID) {
                      var firstInvalid = _this.elementByUID[invalidFormInputUID];
                      if (firstInvalid instanceof HTMLElement) {
                        firstInvalid.focus();
                      }
                    }
                  };
                  this.isValid = function(form, prevalidate, callback) {
                    if (prevalidate === void 0) {
                      prevalidate = true;
                    }
                    if (!(form instanceof HTMLFormElement)) {
                      throw new Error("isValid() can only be called on <form> elements");
                    }
                    if (prevalidate) {
                      _this.validateForm(form, callback);
                    }
                    var formUID = _this.getElementUID(form);
                    var formInputUIDs = _this.formInputs[formUID];
                    var formIsInvalid = (formInputUIDs === null || formInputUIDs === void 0 ? void 0 : formInputUIDs.some(function(uid) {
                      return _this.summary[uid];
                    })) === true;
                    return !formIsInvalid;
                  };
                  this.isFieldValid = function(field, prevalidate, callback) {
                    if (prevalidate === void 0) {
                      prevalidate = true;
                    }
                    if (prevalidate) {
                      _this.validateField(field, callback);
                    }
                    var fieldUID = _this.getElementUID(field);
                    return _this.summary[fieldUID] === void 0;
                  };
                  this.options = {
                    root: document.body,
                    watch: false,
                    addNoValidate: true
                  };
                  this.ValidationInputCssClassName = "input-validation-error";
                  this.ValidationInputValidCssClassName = "input-validation-valid";
                  this.ValidationMessageCssClassName = "field-validation-error";
                  this.ValidationMessageValidCssClassName = "field-validation-valid";
                  this.ValidationSummaryCssClassName = "validation-summary-errors";
                  this.ValidationSummaryValidCssClassName = "validation-summary-valid";
                  this.logger = logger || nullLogger;
                }
                ValidationService2.prototype.addProvider = function(name2, callback) {
                  if (this.providers[name2]) {
                    return;
                  }
                  this.logger.log("Registered provider: %s", name2);
                  this.providers[name2] = callback;
                };
                ValidationService2.prototype.addMvcProviders = function() {
                  var mvc = new MvcValidationProviders();
                  this.addProvider("required", mvc.required);
                  this.addProvider("length", mvc.stringLength);
                  this.addProvider("maxlength", mvc.stringLength);
                  this.addProvider("minlength", mvc.stringLength);
                  this.addProvider("equalto", mvc.compare);
                  this.addProvider("range", mvc.range);
                  this.addProvider("regex", mvc.regex);
                  this.addProvider("creditcard", mvc.creditcard);
                  this.addProvider("email", mvc.email);
                  this.addProvider("url", mvc.url);
                  this.addProvider("phone", mvc.phone);
                  this.addProvider("remote", mvc.remote);
                };
                ValidationService2.prototype.scanMessages = function(root, cb) {
                  var validationMessageElements = Array.from(root.querySelectorAll("span[form]"));
                  for (var _i = 0, validationMessageElements_1 = validationMessageElements; _i < validationMessageElements_1.length; _i++) {
                    var span = validationMessageElements_1[_i];
                    var form = document.getElementById(span.getAttribute("form"));
                    if (form instanceof HTMLFormElement) {
                      cb.call(this, form, span);
                    }
                  }
                  var forms = Array.from(root.querySelectorAll("form"));
                  if (root instanceof HTMLFormElement) {
                    forms.push(root);
                  }
                  var containingForm = root instanceof Element ? root.closest("form") : null;
                  if (containingForm) {
                    forms.push(containingForm);
                  }
                  for (var _a = 0, forms_1 = forms; _a < forms_1.length; _a++) {
                    var form = forms_1[_a];
                    var validationMessageElements_3 = Array.from(form.querySelectorAll("[data-valmsg-for]"));
                    for (var _b = 0, validationMessageElements_2 = validationMessageElements_3; _b < validationMessageElements_2.length; _b++) {
                      var span = validationMessageElements_2[_b];
                      cb.call(this, form, span);
                    }
                  }
                };
                ValidationService2.prototype.pushValidationMessageSpan = function(form, span) {
                  var _a, _b;
                  var _c;
                  var formId = this.getElementUID(form);
                  var formSpans = (_a = (_c = this.messageFor)[formId]) !== null && _a !== void 0 ? _a : _c[formId] = {};
                  var messageForId = span.getAttribute("data-valmsg-for");
                  if (!messageForId)
                    return;
                  var spans = (_b = formSpans[messageForId]) !== null && _b !== void 0 ? _b : formSpans[messageForId] = [];
                  if (spans.indexOf(span) < 0) {
                    spans.push(span);
                  } else {
                    this.logger.log("Validation element for '%s' is already tracked", name, span);
                  }
                };
                ValidationService2.prototype.removeValidationMessageSpan = function(form, span) {
                  var formId = this.getElementUID(form);
                  var formSpans = this.messageFor[formId];
                  if (!formSpans)
                    return;
                  var messageForId = span.getAttribute("data-valmsg-for");
                  if (!messageForId)
                    return;
                  var spans = formSpans[messageForId];
                  if (!spans) {
                    return;
                  }
                  var index = spans.indexOf(span);
                  if (index >= 0) {
                    spans.splice(index, 1);
                  } else {
                    this.logger.log("Validation element for '%s' was already removed", name, span);
                  }
                };
                ValidationService2.prototype.parseDirectives = function(attributes) {
                  var directives = {};
                  var validationAtributes = {};
                  var cut = "data-val-".length;
                  for (var i = 0; i < attributes.length; i++) {
                    var a = attributes[i];
                    if (a.name.indexOf("data-val-") === 0) {
                      var key = a.name.substr(cut);
                      validationAtributes[key] = a.value;
                    }
                  }
                  var _loop_1 = function(key2) {
                    if (key2.indexOf("-") === -1) {
                      var parameters = Object.keys(validationAtributes).filter(function(Q) {
                        return Q !== key2 && Q.indexOf(key2) === 0;
                      });
                      var directive = {
                        error: validationAtributes[key2],
                        params: {}
                      };
                      var pcut = (key2 + "-").length;
                      for (var i2 = 0; i2 < parameters.length; i2++) {
                        var pvalue = validationAtributes[parameters[i2]];
                        var pkey = parameters[i2].substr(pcut);
                        directive.params[pkey] = pvalue;
                      }
                      directives[key2] = directive;
                    }
                  };
                  for (var key in validationAtributes) {
                    _loop_1(key);
                  }
                  return directives;
                };
                ValidationService2.prototype.guid4 = function() {
                  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function(c) {
                    var r = Math.random() * 16 | 0, v = c == "x" ? r : r & 3 | 8;
                    return v.toString(16);
                  });
                };
                ValidationService2.prototype.getElementUID = function(node) {
                  var x = this.elementUIDs.filter(function(e) {
                    return e.node === node;
                  })[0];
                  if (x) {
                    return x.uid;
                  }
                  var uid = this.guid4();
                  this.elementUIDs.push({
                    node,
                    uid
                  });
                  this.elementByUID[uid] = node;
                  return uid;
                };
                ValidationService2.prototype.getFormValidationTask = function(formUID) {
                  var formInputUIDs = this.formInputs[formUID];
                  if (!formInputUIDs || formInputUIDs.length === 0) {
                    return Promise.resolve(true);
                  }
                  var formValidators = [];
                  for (var _i = 0, formInputUIDs_1 = formInputUIDs; _i < formInputUIDs_1.length; _i++) {
                    var inputUID = formInputUIDs_1[_i];
                    var validator = this.validators[inputUID];
                    if (validator) {
                      formValidators.push(validator);
                    }
                  }
                  var tasks = formValidators.map(function(factory) {
                    return factory();
                  });
                  return Promise.all(tasks).then(function(result) {
                    return result.every(function(e) {
                      return e;
                    });
                  });
                };
                ValidationService2.prototype.getMessageFor = function(input) {
                  var _a;
                  if (!input.form) {
                    return void 0;
                  }
                  var formId = this.getElementUID(input.form);
                  return (_a = this.messageFor[formId]) === null || _a === void 0 ? void 0 : _a[input.name];
                };
                ValidationService2.prototype.shouldValidate = function(e) {
                  return !(e && e["submitter"] && e["submitter"]["formNoValidate"]);
                };
                ValidationService2.prototype.trackFormInput = function(form, inputUID) {
                  var _this = this;
                  var _a;
                  var _b;
                  var formUID = this.getElementUID(form);
                  var formInputUIDs = (_a = (_b = this.formInputs)[formUID]) !== null && _a !== void 0 ? _a : _b[formUID] = [];
                  var add = formInputUIDs.indexOf(inputUID) === -1;
                  if (add) {
                    formInputUIDs.push(inputUID);
                    if (this.options.addNoValidate) {
                      this.logger.log("Setting novalidate on form", form);
                      form.setAttribute("novalidate", "novalidate");
                    } else {
                      this.logger.log("Not setting novalidate on form", form);
                    }
                  } else {
                    this.logger.log("Form input for UID '%s' is already tracked", inputUID);
                  }
                  if (this.formEvents[formUID]) {
                    return;
                  }
                  var validationTask = null;
                  var cb = function(e, callback) {
                    if (validationTask) {
                      return validationTask;
                    }
                    if (!_this.shouldValidate(e)) {
                      return Promise.resolve(true);
                    }
                    validationTask = _this.getFormValidationTask(formUID);
                    if (e) {
                      _this.preValidate(e);
                    }
                    _this.logger.log("Validating", form);
                    return validationTask.then(function(success) {
                      return __awaiter(_this, void 0, void 0, function() {
                        var validationEvent;
                        return __generator(this, function(_a2) {
                          switch (_a2.label) {
                            case 0:
                              this.logger.log("Validated (success = %s)", success, form);
                              if (callback) {
                                callback(success);
                                return [2, success];
                              }
                              validationEvent = new CustomEvent("validation", {
                                detail: { valid: success }
                              });
                              form.dispatchEvent(validationEvent);
                              return [4, new Promise(function(resolve) {
                                return setTimeout(resolve, 0);
                              })];
                            case 1:
                              _a2.sent();
                              this.handleValidated(form, success, e);
                              return [2, success];
                          }
                        });
                      });
                    }).catch(function(error) {
                      _this.logger.log("Validation error", error);
                      return false;
                    }).finally(function() {
                      validationTask = null;
                    });
                  };
                  form.addEventListener("submit", cb);
                  var cbReset = function(e) {
                    var formInputUIDs2 = _this.formInputs[formUID];
                    for (var _i = 0, formInputUIDs_2 = formInputUIDs2; _i < formInputUIDs_2.length; _i++) {
                      var inputUID_1 = formInputUIDs_2[_i];
                      _this.resetField(inputUID_1);
                    }
                    _this.renderSummary();
                  };
                  form.addEventListener("reset", cbReset);
                  cb.remove = function() {
                    form.removeEventListener("submit", cb);
                    form.removeEventListener("reset", cbReset);
                  };
                  this.formEvents[formUID] = cb;
                };
                ValidationService2.prototype.reset = function(input) {
                  if (this.isDisabled(input)) {
                    this.resetField(this.getElementUID(input));
                  } else {
                    this.scan(input);
                  }
                };
                ValidationService2.prototype.resetField = function(inputUID) {
                  var input = this.elementByUID[inputUID];
                  this.swapClasses(input, "", this.ValidationInputCssClassName);
                  this.swapClasses(input, "", this.ValidationInputValidCssClassName);
                  var spans = isValidatable(input) && this.getMessageFor(input);
                  if (spans) {
                    for (var i = 0; i < spans.length; i++) {
                      spans[i].innerHTML = "";
                      this.swapClasses(spans[i], "", this.ValidationMessageCssClassName);
                      this.swapClasses(spans[i], "", this.ValidationMessageValidCssClassName);
                    }
                  }
                  delete this.summary[inputUID];
                };
                ValidationService2.prototype.untrackFormInput = function(form, inputUID) {
                  var _a;
                  var formUID = this.getElementUID(form);
                  var formInputUIDs = this.formInputs[formUID];
                  if (!formInputUIDs) {
                    return;
                  }
                  var indexToRemove = formInputUIDs.indexOf(inputUID);
                  if (indexToRemove >= 0) {
                    formInputUIDs.splice(indexToRemove, 1);
                    if (!formInputUIDs.length) {
                      (_a = this.formEvents[formUID]) === null || _a === void 0 ? void 0 : _a.remove();
                      delete this.formEvents[formUID];
                      delete this.formInputs[formUID];
                      delete this.messageFor[formUID];
                    }
                  } else {
                    this.logger.log("Form input for UID '%s' was already removed", inputUID);
                  }
                };
                ValidationService2.prototype.addInput = function(input) {
                  var _this = this;
                  var _a;
                  var uid = this.getElementUID(input);
                  var directives = this.parseDirectives(input.attributes);
                  this.validators[uid] = this.createValidator(input, directives);
                  if (input.form) {
                    this.trackFormInput(input.form, uid);
                  }
                  if (this.inputEvents[uid]) {
                    return;
                  }
                  var cb = function(event, callback) {
                    return __awaiter(_this, void 0, void 0, function() {
                      var validate, success, error_1;
                      return __generator(this, function(_a2) {
                        switch (_a2.label) {
                          case 0:
                            validate = this.validators[uid];
                            if (!validate)
                              return [2, true];
                            if (!input.dataset.valEvent && event && event.type === "input" && !input.classList.contains(this.ValidationInputCssClassName)) {
                              return [2, true];
                            }
                            this.logger.log("Validating", { event });
                            _a2.label = 1;
                          case 1:
                            _a2.trys.push([1, 3, , 4]);
                            return [4, validate()];
                          case 2:
                            success = _a2.sent();
                            callback(success);
                            return [2, success];
                          case 3:
                            error_1 = _a2.sent();
                            this.logger.log("Validation error", error_1);
                            return [2, false];
                          case 4:
                            return [
                              2
                              /*return*/
                            ];
                        }
                      });
                    });
                  };
                  var debounceTimeoutID = null;
                  cb.debounced = function(event, callback) {
                    if (debounceTimeoutID !== null) {
                      clearTimeout(debounceTimeoutID);
                    }
                    debounceTimeoutID = setTimeout(function() {
                      cb(event, callback);
                    }, _this.debounce);
                  };
                  var defaultEvent = input instanceof HTMLSelectElement ? "change" : "input change";
                  var validateEvent = (_a = input.dataset.valEvent) !== null && _a !== void 0 ? _a : defaultEvent;
                  var events = validateEvent.split(" ");
                  events.forEach(function(eventName) {
                    input.addEventListener(eventName, cb.debounced);
                  });
                  cb.remove = function() {
                    events.forEach(function(eventName) {
                      input.removeEventListener(eventName, cb.debounced);
                    });
                  };
                  this.inputEvents[uid] = cb;
                };
                ValidationService2.prototype.removeInput = function(input) {
                  var uid = this.getElementUID(input);
                  var cb = this.inputEvents[uid];
                  if (cb === null || cb === void 0 ? void 0 : cb.remove) {
                    cb.remove();
                    delete cb.remove;
                  }
                  delete this.summary[uid];
                  delete this.inputEvents[uid];
                  delete this.validators[uid];
                  if (input.form) {
                    this.untrackFormInput(input.form, uid);
                  }
                };
                ValidationService2.prototype.scanInputs = function(root, cb) {
                  var inputs = Array.from(root.querySelectorAll(validatableSelector('[data-val="true"]')));
                  if (isValidatable(root) && root.getAttribute("data-val") === "true") {
                    inputs.push(root);
                  }
                  for (var i = 0; i < inputs.length; i++) {
                    var input = inputs[i];
                    cb.call(this, input);
                  }
                };
                ValidationService2.prototype.createSummaryDOM = function() {
                  if (!Object.keys(this.summary).length) {
                    return null;
                  }
                  var renderedMessages = [];
                  var ul = document.createElement("ul");
                  for (var key in this.summary) {
                    var matchingElement = this.elementByUID[key];
                    if (matchingElement instanceof HTMLInputElement) {
                      if (matchingElement.type === "checkbox" || matchingElement.type === "radio") {
                        if (matchingElement.className === this.ValidationInputValidCssClassName) {
                          continue;
                        }
                      }
                    }
                    if (renderedMessages.indexOf(this.summary[key]) > -1) {
                      continue;
                    }
                    var li = document.createElement("li");
                    li.innerHTML = this.summary[key];
                    ul.appendChild(li);
                    renderedMessages.push(this.summary[key]);
                  }
                  return ul;
                };
                ValidationService2.prototype.renderSummary = function() {
                  var summaryElements = document.querySelectorAll('[data-valmsg-summary="true"]');
                  if (!summaryElements.length) {
                    return;
                  }
                  var shadow = JSON.stringify(this.summary, Object.keys(this.summary).sort());
                  if (shadow === this.renderedSummaryJSON) {
                    return;
                  }
                  this.renderedSummaryJSON = shadow;
                  var ul = this.createSummaryDOM();
                  for (var i = 0; i < summaryElements.length; i++) {
                    var e = summaryElements[i];
                    var listElements = e.querySelectorAll("ul");
                    for (var j = 0; j < listElements.length; j++) {
                      listElements[j].remove();
                    }
                    if (ul && ul.hasChildNodes()) {
                      this.swapClasses(e, this.ValidationSummaryCssClassName, this.ValidationSummaryValidCssClassName);
                      e.appendChild(ul.cloneNode(true));
                    } else {
                      this.swapClasses(e, this.ValidationSummaryValidCssClassName, this.ValidationSummaryCssClassName);
                    }
                  }
                };
                ValidationService2.prototype.addError = function(input, message) {
                  var spans = this.getMessageFor(input);
                  if (spans) {
                    for (var i = 0; i < spans.length; i++) {
                      spans[i];
                      spans[i].innerHTML = message;
                      this.swapClasses(spans[i], this.ValidationMessageCssClassName, this.ValidationMessageValidCssClassName);
                    }
                  }
                  this.highlight(input, this.ValidationInputCssClassName, this.ValidationInputValidCssClassName);
                  if (input.form) {
                    var inputs = input.form.querySelectorAll(validatableSelector('[name="'.concat(input.name, '"]')));
                    for (var i = 0; i < inputs.length; i++) {
                      this.swapClasses(inputs[i], this.ValidationInputCssClassName, this.ValidationInputValidCssClassName);
                      var uid = this.getElementUID(inputs[i]);
                      this.summary[uid] = message;
                    }
                  }
                  this.renderSummary();
                };
                ValidationService2.prototype.removeError = function(input) {
                  var spans = this.getMessageFor(input);
                  if (spans) {
                    for (var i = 0; i < spans.length; i++) {
                      spans[i].innerHTML = "";
                      this.swapClasses(spans[i], this.ValidationMessageValidCssClassName, this.ValidationMessageCssClassName);
                    }
                  }
                  this.unhighlight(input, this.ValidationInputCssClassName, this.ValidationInputValidCssClassName);
                  if (input.form) {
                    var inputs = input.form.querySelectorAll(validatableSelector('[name="'.concat(input.name, '"]')));
                    for (var i = 0; i < inputs.length; i++) {
                      this.swapClasses(inputs[i], this.ValidationInputValidCssClassName, this.ValidationInputCssClassName);
                      var uid = this.getElementUID(inputs[i]);
                      delete this.summary[uid];
                    }
                  }
                  this.renderSummary();
                };
                ValidationService2.prototype.createValidator = function(input, directives) {
                  var _this = this;
                  return function() {
                    return __awaiter(_this, void 0, void 0, function() {
                      var _a, _b, _c, _i, key, directive, provider, result, valid, error, resolution;
                      return __generator(this, function(_d) {
                        switch (_d.label) {
                          case 0:
                            if (!(!this.isHidden(input) && !this.isDisabled(input))) return [3, 7];
                            _a = directives;
                            _b = [];
                            for (_c in _a)
                              _b.push(_c);
                            _i = 0;
                            _d.label = 1;
                          case 1:
                            if (!(_i < _b.length)) return [3, 7];
                            _c = _b[_i];
                            if (!(_c in _a)) return [3, 6];
                            key = _c;
                            directive = directives[key];
                            provider = this.providers[key];
                            if (!provider) {
                              this.logger.log("aspnet-validation provider not implemented: %s", key);
                              return [3, 6];
                            }
                            this.logger.log("Running %s validator on element", key, input);
                            result = provider(input.value, input, directive.params);
                            valid = false;
                            error = directive.error;
                            if (!(typeof result === "boolean")) return [3, 2];
                            valid = result;
                            return [3, 5];
                          case 2:
                            if (!(typeof result === "string")) return [3, 3];
                            valid = false;
                            error = result;
                            return [3, 5];
                          case 3:
                            return [4, result];
                          case 4:
                            resolution = _d.sent();
                            if (typeof resolution === "boolean") {
                              valid = resolution;
                            } else {
                              valid = false;
                              error = resolution;
                            }
                            _d.label = 5;
                          case 5:
                            if (!valid) {
                              this.addError(input, error);
                              return [2, false];
                            }
                            _d.label = 6;
                          case 6:
                            _i++;
                            return [3, 1];
                          case 7:
                            this.removeError(input);
                            return [2, true];
                        }
                      });
                    });
                  };
                };
                ValidationService2.prototype.isHidden = function(input) {
                  return !(this.allowHiddenFields || input.offsetWidth || input.offsetHeight || input.getClientRects().length);
                };
                ValidationService2.prototype.isDisabled = function(input) {
                  return input.disabled;
                };
                ValidationService2.prototype.swapClasses = function(element, addClass, removeClass) {
                  if (addClass && !this.isDisabled(element) && !element.classList.contains(addClass)) {
                    element.classList.add(addClass);
                  }
                  if (element.classList.contains(removeClass)) {
                    element.classList.remove(removeClass);
                  }
                };
                ValidationService2.prototype.bootstrap = function(options) {
                  var _this = this;
                  Object.assign(this.options, options);
                  this.addMvcProviders();
                  var document2 = window.document;
                  var root = this.options.root;
                  var init = function() {
                    _this.scan(root);
                    if (_this.options.watch) {
                      _this.watch(root);
                    }
                  };
                  if (document2.readyState === "complete" || document2.readyState === "interactive") {
                    init();
                  } else {
                    document2.addEventListener("DOMContentLoaded", init);
                  }
                };
                ValidationService2.prototype.scan = function(root) {
                  root !== null && root !== void 0 ? root : root = this.options.root;
                  this.logger.log("Scanning", root);
                  this.scanMessages(root, this.pushValidationMessageSpan);
                  this.scanInputs(root, this.addInput);
                };
                ValidationService2.prototype.remove = function(root) {
                  root !== null && root !== void 0 ? root : root = this.options.root;
                  this.logger.log("Removing", root);
                  this.scanMessages(root, this.removeValidationMessageSpan);
                  this.scanInputs(root, this.removeInput);
                };
                ValidationService2.prototype.watch = function(root) {
                  var _this = this;
                  root !== null && root !== void 0 ? root : root = this.options.root;
                  this.observer = new MutationObserver(function(mutations) {
                    mutations.forEach(function(mutation) {
                      _this.observed(mutation);
                    });
                  });
                  this.observer.observe(root, {
                    attributes: true,
                    childList: true,
                    subtree: true
                  });
                  this.logger.log("Watching for mutations");
                };
                ValidationService2.prototype.observed = function(mutation) {
                  var _a, _b, _c;
                  if (mutation.type === "childList") {
                    for (var i = 0; i < mutation.addedNodes.length; i++) {
                      var node = mutation.addedNodes[i];
                      this.logger.log("Added node", node);
                      if (node instanceof HTMLElement) {
                        this.scan(node);
                      }
                    }
                    for (var i = 0; i < mutation.removedNodes.length; i++) {
                      var node = mutation.removedNodes[i];
                      this.logger.log("Removed node", node);
                      if (node instanceof HTMLElement) {
                        this.remove(node);
                      }
                    }
                  } else if (mutation.type === "attributes") {
                    if (mutation.target instanceof HTMLElement) {
                      var attributeName = mutation.attributeName;
                      if (attributeName === "disabled") {
                        var target = mutation.target;
                        this.reset(target);
                      } else {
                        var oldValue = (_a = mutation.oldValue) !== null && _a !== void 0 ? _a : "";
                        var newValue = (_c = (_b = mutation.target.attributes[mutation.attributeName]) === null || _b === void 0 ? void 0 : _b.value) !== null && _c !== void 0 ? _c : "";
                        this.logger.log("Attribute '%s' changed from '%s' to '%s'", mutation.attributeName, oldValue, newValue, mutation.target);
                        if (oldValue !== newValue) {
                          this.scan(mutation.target);
                        }
                      }
                    }
                  }
                };
                ValidationService2.prototype.highlight = function(input, errorClass, validClass) {
                  this.swapClasses(input, errorClass, validClass);
                };
                ValidationService2.prototype.unhighlight = function(input, errorClass, validClass) {
                  this.swapClasses(input, validClass, errorClass);
                };
                return ValidationService2;
              })()
            );
            return __webpack_exports__;
          })()
        );
      });
    })(aspnetValidation);
    return aspnetValidation.exports;
  }
  var aspnetValidationExports = requireAspnetValidation();
  if (!document.body.attributes.__htmx_antiforgery) {
    document.addEventListener("htmx:configRequest", (evt) => {
      const { verb, parameters, headers } = evt.detail;
      if (verb?.toUpperCase() === "GET") return;
      const antiforgery = htmx.config?.antiforgery;
      if (!antiforgery) return;
      const { headerName, requestToken, formFieldName } = antiforgery;
      if (!headerName && !formFieldName) {
        console.warn("Antiforgery configuration is missing both headerName and formFieldName. Token not added.");
        return;
      }
      if (formFieldName && parameters[formFieldName]) return;
      if (headerName) {
        headers[headerName] = requestToken;
      } else {
        parameters[formFieldName] = requestToken;
      }
    });
    document.addEventListener("htmx:afterOnLoad", (evt) => {
      if (evt.detail.boosted) {
        const responseText = evt.detail.xhr.responseText;
        const selector = "meta[name=htmx-config]";
        const startIndex = responseText.indexOf(`<meta name="htmx-config"`);
        const endIndex = responseText.indexOf(">", startIndex) + 1;
        const closingTagIndex = responseText.indexOf("</meta>", endIndex);
        if (startIndex > -1) {
          let metaTagString = "";
          if (closingTagIndex > -1) {
            const closingSlashIndex = responseText.indexOf("/", startIndex);
            if (closingSlashIndex > -1 && closingSlashIndex < endIndex) {
              metaTagString = responseText.substring(startIndex, endIndex);
            } else {
              metaTagString = responseText.substring(startIndex, responseText.indexOf("</meta>") + 7);
            }
          } else {
            metaTagString = responseText.substring(startIndex, endIndex);
          }
          const contentAttributeRegex = /content="([^"]*)"/;
          const contentMatch = metaTagString.match(contentAttributeRegex);
          if (contentMatch && contentMatch[1]) {
            const contentValue = contentMatch[1];
            const current = document.querySelector(selector);
            const key = "antiforgery";
            try {
              htmx.config[key] = JSON.parse(contentValue)[key];
            } catch (e) {
              console.error("Error parsing htmx-config JSON:", e);
              return;
            }
            const newMeta = document.createElement("meta");
            newMeta.setAttribute("name", "htmx-config");
            newMeta.setAttribute("content", contentValue);
            current.replaceWith(newMeta);
          }
        }
      }
    });
    document.body.attributes.__htmx_antiforgery = true;
  }
  let validation = new aspnetValidationExports.ValidationService();
  validation.bootstrap({ watch: true });
  const Rizzy = {
    validation
  };
  window.Rizzy = { ...window.Rizzy || {}, ...Rizzy };
  return Rizzy;
}));
//# sourceMappingURL=rizzy.js.map
