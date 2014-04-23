/// <reference path="/LiveSDKHTML/js/wl.js" />
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            liveSdkSample.currentPage = liveSdkSample.HOMEPAGE;
            WL.getLoginStatus().then(
                    function (result) {
                        if (result.status == "connected") {
                            statusText.innerText = "You are connected! ";
                            liveSdkSample.displayMe();
                        }
                        else {
                            statusText.innerText = "You are not connected! ";
                            connectButton.onclick = function () {
                                WL.login({
                                    scope: liveSdkSample.defaultScopes
                                }).then(
                                    function (result) {
                                        if (result.status == "connected") {
                                            connectButton.style.display = "none";
                                            statusText.innerText = "You are connected!";
                                            liveSdkSample.displayMe();
                                        }
                                    }
                                );
                            };
                            connectButton.style.display = "block";
                        }
                    });
        },

        
    });
})();

