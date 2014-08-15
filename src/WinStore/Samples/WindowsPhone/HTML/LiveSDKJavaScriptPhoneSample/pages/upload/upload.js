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
