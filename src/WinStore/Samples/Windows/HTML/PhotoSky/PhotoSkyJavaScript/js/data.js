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

(function () {
    "use strict";

    // Get a reference for an item, using the group key and item title as a
    // unique reference to the item that can be easily serialized.
    function getItemReference(item) {
        return [item.group.key, item.title];
    }

    function resolveGroupReference(key) {
        for (var i = 0; i < groupedItems.groups.length; i++) {
            if (groupedItems.groups.getAt(i).key === key) {
                return groupedItems.groups.getAt(i);
            }
        }
    }

    function resolveItemReference(reference) {
        for (var i = 0; i < groupedItems.length; i++) {
            var item = groupedItems.getAt(i);
            if (item.group.key === reference[0] && item.title === reference[1]) {
                return item;
            }
        }
    }

    // This function returns a WinJS.Binding.List containing only the items
    // that belong to the provided group.
    function getItemsFromGroup(group) {
        return list.createFiltered(function (item) { return item.group.key === group.key; });
    }

    var list = new WinJS.Binding.List();
    var groupedItems = list.createGrouped(
        function groupKeySelector(item) { return item.group.key; },
        function groupDataSelector(item) { return item.group; }
    );

    WL.Event.subscribe("auth.login", onLoginComplete);
    function onLoginComplete() {
        var session = WL.getSession();
        if (!session.error) {
            getUserPhotos();
        }
    };

    function getUserPhotos() {
        var albums_path = "/me/albums";
        WL.api({ path: albums_path, method: "GET" }).then(function (response) {
            getPictures(response);
        });

    }

    var groups = [];
    function getPictures(result) {
        if (result.error) {
        }
        else {
            var items = result.data
            for (var index in items) {
                downloadPicture(items[index].id);
                groups.push({
                    key: items[index].id,
                    title: items[index].name,
                    subtitle: items[index].name,
                    description: items[index].type
                });
            }
        }
    };
    function downloadPicture(folderId) {

        var path = folderId + "/files"

        // Submit request
        WL.api({ path: path, method: "GET" }).then(function (response) {
            loadPhotos(response);
        });

    };
    function loadPhotos(result) {
        if (result.error) {

        }
        else {
            var msg;
            var items = result.data
            for (var index in items) {
                var parentGroup;
                for (var g = 0, gl = groups.length; g < gl; g++) {
                    if (groups[g].key === items[index].parent_id) {
                        parentGroup = groups[g];
                        break;
                    }
                }

                list.push({
                    group: parentGroup,
                    key: items[index].id,
                    title: items[index].name,
                    subtitle: items[index].description,
                    backgroundImage: items[index].source,
                    content: '',
                    description: ''
                });

            }

        }
    };

    WinJS.Namespace.define("Data", {
        items: groupedItems,
        groups: groupedItems.groups,
        getItemsFromGroup: getItemsFromGroup,
        getItemReference: getItemReference,
        resolveGroupReference: resolveGroupReference,
        resolveItemReference: resolveItemReference
    });


})();
