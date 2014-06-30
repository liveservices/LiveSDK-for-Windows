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