<!-- --------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// ------------------------------------------------------------------------ -->

// For an introduction to the Page Control template, see the following documentation:
// TODO: FWLINK
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/apiLab/apiLab.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            liveSdkSample.currentPage = liveSdkSample.APILABPAGE;
            attachDomEvents();
            onMethodChange();
        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in layout.
        }
    });



    function attachDomEvents() {
        id("clear").onclick = clearOutput;
        id("sendRequest").onclick = sendAPIServiceRequest;
        id("apiBody").onfocusout = showCode;
        id("apiMethod").onchange = onMethodChange;
        id("apiPath").onchange = showCode;
        destinationPath.onchange = showCode;
    }

    function onMethodChange() {
        var disableApiBody = true,
            disableDestinationPath = true;

        switch (getApiMethod()) {
            case "MOVE":
            case "COPY":
                disableDestinationPath = false;
                break;
            case "PUT":
            case "POST":
                disableApiBody = false;
                break;
            default:
                break;
        }

        apiBody.disabled = disableApiBody;
        destinationPath.disabled = disableDestinationPath;

        showCode();
    }

    function showCode() {

        var method = getApiMethod(),
            path = id("apiPath").value,
            authScope = getAuthScope(),
            codeBlock = id("code");

        if (path.trim() == "") {
            codeBlock.value = "The 'path' value is required.";
            codeBlock.style.color = "red";
            return;
        }

        codeBlock.style.color = "blue";

        showApiCode(method, path, authScope);
    }

    function showApiCode(method, path, scope) {
        var jsonBody = null;
        var requestParams = {
            method: method,
            path: path
        };

        var codeBlock = id("code");

        switch (method) {
            case "COPY":
            case "MOVE":
                if (destinationPath.value.trim() == "") {
                    codeBlock.value = "The 'destination' value is required for COPY and MOVE requests";
                    codeBlock.style.color = "red";
                    return;
                }
                else {
                    jsonBody = { destination: destinationPath.value };
                    apiBody.value = JSON.stringify(jsonBody, null, "\t");
                    requestParams.body = jsonBody;
                }
                break;
            case "PUT":
            case "POST":
                jsonBody = getBody(method);
                if (jsonBody) {
                    requestParams.body = jsonBody;
                }
                else {
                    codeBlock.value = "The request body is not in JSON format. Please correct it.";
                    codeBlock.style.color = "red";
                    return;
                }
                break;
            default:
                break;
        }

        codeBlock.value = invokeApi.toString() +
            "\ninvokeApi(" + window.JSON.stringify(requestParams, null, "\t") + ", " +
            "\n\t'" + scope + "'" +
            "\n});";
    }

    function getApiMethod() {
        var apiMethodBox = id("apiMethod");
        return apiMethodBox[apiMethodBox.selectedIndex].text;
    }

    function getAuthScope() {
        var authScope = id("apiScope");
        return authScope[authScope.selectedIndex].text;
    }

    function sendAPIServiceRequest() {

        var path = id("apiPath").value,
            method = getApiMethod(),
            authScope = getAuthScope();

        log("[api-request](" + path + "," + method + ")");
        logSeperatorLine();

        prepareAndInvokeApi(path, method, authScope);
    }

    function getBody(method) {
        switch (method) {
            case "PUT":
            case "POST":
            case "MOVE":
            case "COPY":
                return liveSdkSample.parseJson(apiBody.value);
            default:
                return null;
        }
    }

    function prepareAndInvokeApi(path, method, scope) {
        var body = getBody(method),
            requestProps = {
                path: path,
                method: method
            };

        if (body) {
            requestProps.body = body;
        }

        invokeApi(requestProps, scope);
    }

    function invokeApi(requestParams, scope) {
        WL.login({ scope: scope }).then(
            function (result) {
                WL.api(requestParams).then(
                    function (response) {
                        logObject("api_response", response);
                    },
                    function (response) {
                        logObject("api_response_error", response);
                    }
                );
            },
            function (result) {
                logObject("api_auth_error", result);
            });
    }
})();
