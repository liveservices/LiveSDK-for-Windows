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


/// logging functions ///
function clearOutput() {
    id("outputbox").value = "";
}

function log(text) {
    id("outputbox").value += text + "\r\n";
}

function logSeperatorLine() {
    log("------------------" + new Date().toTimeString() + "--------------------");
}

function logObject(name, obj) {
    log(name + " : ");
    log(window.JSON.stringify(obj, null, "\t"));
    logSeperatorLine();
}