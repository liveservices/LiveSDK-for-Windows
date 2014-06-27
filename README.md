LiveSDK for Windows and Windows Phone
================
version 5.6

1. Introduction
---------------

The Live SDK for Windows and Windows Phone library is intended to help developers to easily 
integrate OneDrive and Outlook.com contacts into their Windows Runtime apps (HTML and XAML). 

The Live SDK enables access to users’ identity profile info and allows your apps to:
* Upload and download photos, videos, documents, and other files in OneDrive
* Personalize your users’ experience
* Take advantage of single sign-on using Microsoft Account in Windows 8.1 and Windows Phone 8.1
* Create, read, and update contacts, calendars, and events in Outlook.com

2. Reference the SDK 
--------------------
* Install the Live SDK from the [download site](http://www.microsoft.com/en-us/download/details.aspx?id=42552)
* Open your app's project in Visual Studio
* In Solution Explorer, right-click References > Add Reference
* For Windows Runtime HTML apps: Click Windows > Extension SDKs and select both Live SDK and Windows Library for JavaScript 2.1. Further details available [here](http://msdn.microsoft.com/en-us/library/dn631820.aspx)
* For Windows Runtime XAML apps: Click Windows 8.1 or Windows Phone 8.1 > Extensions and select Live SDK. Further details available [here](http://msdn.microsoft.com/en-us/library/dn631823.aspx)

To use the SDK in a Windows Store app with single sign-in (the default), you need to register the application with the Windows Store. Failing to reigster the app will generate a runtime exception when logging in with the SDK. For more information about how to register with the Windows Store, click [here](http://msdn.microsoft.com/en-US/library/dn631823.aspx#associate_your_app_with_the_store)


3. Documentation
----------------

Visit: http://dev.onedrive.com. 

4. Sample code
-----------------
[Windows Store App Samples](https://github.com/liveservices/LiveSDK-for-Windows/tree/master/src/WinStore)
* Includes samples for HTML and XAML apps for Windows and Windows Phone

[Desktop Samples](https://github.com/liveservices/LiveSDK-for-Windows/tree/master/src/Desktop)
* API Explorer: Authenticates user and uploads and downloads files

[Web Samples](https://github.com/liveservices/LiveSDK-for-Windows/tree/master/src/Web)
* 3 samples that demonstrate authentication and OAuth with ASP.Net or PHP

[Windows Phone 8 Samples](https://github.com/liveservices/LiveSDK-for-Windows/tree/master/src/WP8)
* 5 samples that demonstrate authentication and core concepts (upload, download, etc)
