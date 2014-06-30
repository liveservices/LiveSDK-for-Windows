<!-- ------------------------------------------------------------------------------
// Copyright 2014 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ------------------------------------------------------------------------------ -->

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
