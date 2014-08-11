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

/// <reference path="///LiveSDKHTML/js/wl.js" />
// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";
    
    var app = WinJS.Application;
    
    // This function responds to all application activations.
    app.onactivated = function (eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // TODO: Initialize your application here.
            WinJS.UI.processAll();

           WL.getCurrentBackgroundDownloads().then(
                function (pendingOps) {
                    for (var i = 0; i < pendingOps.length; i++) {
                        pendingOps[i].attach().then(
                            function (result) {
                                // Handle pending operation succeeded
                            },
                            function (result) {
                                // Handle pending operation failure
                            },
                            function (result) {
                                // Handle pending operation progress
                            });
                    }
                },
                function (error) {
                });
            WL.getCurrentBackgroundUploads().then(
                function (pendingOps) {
                    for (var i = 0; i < pendingOps.length; i++) {
                        pendingOps[i].attach().then(
                            function (result) {
                                // Handle pending operation succeeded
                            },
                            function (result) {
                                // Handle pending operation failure
                            },
                            function (result) {
                                // Handle pending operation progress
                            });
                    }
                });
        }
    };
    
    app.start();
})();

function nav(location) {
    WinJS.Navigation.navigate(location);
}