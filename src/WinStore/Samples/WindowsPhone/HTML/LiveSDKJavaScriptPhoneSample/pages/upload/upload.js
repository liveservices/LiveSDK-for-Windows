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

    WinJS.UI.Pages.define("/pages/upload/upload.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            liveSdkSample.currentPage = liveSdkSample.UPLOADPAGE;
            id("clear").onclick = clearOutput;
            id("send-request").onclick = runUpload;
            id("upload-path").value = "me/skydrive";            
        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in layout.
        }
    });

    function runUpload() {
        WL.login({
                scope: ["wl.skydrive_update"]
        }).then(
            upload,
            function(result)                
            {
                logObject("consent failure", result);
            }
        );
    }

    function upload()
    {
        var imgPath = id("image-source-uri").value,
            uploadPath = id("upload-path").value,
            fileName = "imgfrominternet.jpg";

        if (imgPath.trim() === "" || fileName.trim() === "" || uploadPath.trim() === "") {
            return;
        }

        var uri = new Windows.Foundation.Uri(imgPath),
            downloader = Windows.Networking.BackgroundTransfer.BackgroundDownloader();

        downloader.createDownload(uri, null).startAsync().then(
            function (result) {
                var stream = result.getResultStreamAt(0);
                WL.createBackgroundUpload({
                    path: uploadPath,
                    stream_input: stream,
                    file_name: fileName,
                    overwrite: true
                }).then(
                    function (result) {
                        return result.start();
                    },
                    function (result) {
                        logObject("upload failure", result);
                    }
                ).then(
                    function (result) {
                        logObject("Upload success", result);

                        WL.api({ path: result.id + "/picture", method: "get" }).then(
                            function (response) {
                                if (response.location) {
                                    id("img-file").innerHTML = "<img src='" + response.location + "' style='height:150px'  />";
                                }
                            },
                            function (failedResponse) {
                                logObject("Getting OneDrive file failure", failedResponse);
                            });
                    },
                    function (result) {
                        logObject("Upload failure", result);
                    },
                    function (progress) {
                        logObject("Upload progress", progress);
                    });

            },
            function (result) {
                logObject("download error", result);
            });
    }
})();
