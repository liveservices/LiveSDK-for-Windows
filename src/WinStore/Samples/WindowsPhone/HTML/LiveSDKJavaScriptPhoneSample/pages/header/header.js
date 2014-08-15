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

    WinJS.UI.Pages.define("/pages/header/header.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            id("button-auth-status").onclick = function () {
                if (liveSdkSample.currentPage != liveSdkSample.HOMEPAGE) {
                    WinJS.Navigation.navigate(liveSdkSample.HOMEPAGE);
                }
            };

            id("button-nav-api").onclick = function () {
                if (liveSdkSample.currentPage != liveSdkSample.APILABPAGE) {
                    WinJS.Navigation.navigate(liveSdkSample.APILABPAGE);
                }
            };

            id("button-upload").onclick = function () {
                if (liveSdkSample.currentPage != liveSdkSample.UPLOADPAGE) {
                    WinJS.Navigation.navigate(liveSdkSample.UPLOADPAGE);
                }
            };

            liveSdkSample.init();

        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in layout.
        }
    });


    window.id = function (objId) {
        return document.getElementById(objId);
    };


    window.liveSdkSample = {
        HOMEPAGE: "/pages/home/home.html",
        APILABPAGE: "/pages/apilab/apilab.html",
        UPLOADPAGE: "/pages/upload/upload.html",
        defaultScopes: ["wl.signin", "wl.skydrive"],
        init: function () {
            WL.init({ scope: liveSdkSample.defaultScopes }).then(
                function (result) {
                    if (result.status == "connected") {
                        liveSdkSample.displayMe();
                    }
                    else {
                        connectButton.onclick = function () {
                            WL.login({
                                scope: liveSdkSample.defaultScopes
                            }).then(
                                function (result) {
                                    if (result.status == "connected") {
                                        liveSdkSample.displayMe();
                                    }
                                }
                            );
                        };
                    }
                });
        },

        displayMe: function () {
            var imgHolder = id("profile-img"),
                nameHolder = id("profile-name");

            if (WL.getSession() != null) {
                WL.api({ path: "me/picture", method: "get" }).then(
                        function (response) {
                            if (response.location) {
                                imgHolder.innerHTML = "<img src='" + response.location + "' style='width:80px;height:80px'  />";
                            }
                        },
                        function (failedResponse) {
                            logObject("get-me/picture failure", failedResponse);
                        }
                    );

                WL.api({ path: "me", method: "get" }).then(
                        function (response) {
                            nameHolder.innerHTML = response.first_name;
                        },
                        function (failedResponse) {
                            logObject("get-me failure", failedResponse);
                        }
                    );
            }
        },

        parseJson: function (value) {
            try {
                return JSON.parse(value);
            }
            catch (error) {
                return null;
            }
        },

        serializeParameters: function (dict) {
            var serialized = "";
            if (dict != null) {
                for (var key in dict) {
                    var separator = (serialized.length == 0) ? "" : "&";
                    var value = dict[key];
                    serialized += separator + encodeURIComponent(key) + "=" + encodeURIComponent(value);
                }
            }

            return serialized;
        },

        deserializeParameters: function (value) {
            var dict = {};

            if (value != null) {
                var properties = value.split('&');
                for (var i = 0; i < properties.length; i++) {
                    var property = properties[i].split('=');
                    if (property.length == 2) {
                        dict[decodeURIComponent(property[0])] = decodeURIComponent(property[1]);
                    }
                }
            }

            return dict;
        }
    };
})();


/// logging functions ///
function clearOutput() {
    var output = id("outputbox");
    if (output) {
        output.value = "";
    }
}

function log(text) {
    var output = id("outputbox");
    if (output) {
        output.value += text + "\r\n";
    }
}

function logSeperatorLine() {
    log("----" + new Date().toISOString() + "----");
}

function logObject(name, obj) {
    log(name + " : ");
    log(window.JSON.stringify(obj, null, "\t"));
    logSeperatorLine();
}