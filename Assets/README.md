# Fyber's Unity Plugin

For more information about the plugin, please refer to [Integrating Fyber Unity Plugin](http://developer.fyber.com/content/current/unity).


# Changelog - Unity SDK

## 8.1.5 - (04/2016)

### Fixes

* Issue preventing mediation to be started in the first build (Android)

## 8.1.4 - (04/2016)

### Update

* Native Android SDK to 8.1.6

### Fixes

* Orientation issues when building with some specific orientation settings
* Collisions between dlls due to their names
* Build error when targeting platforms the Fyber Plugin does not support

## 8.1.2 - (03/2016)

### Update

* Native iOS SDK to 8.2.1
* Native Android SDK to 8.1.5

### Fixes

* Possible issues importing Unity pacakges on Unity 5.3
* Possible issue with check AndroidManifest feature on Unity 5.3
* Possible compilation issue on Unity 4.7

## 8.1.1 - (02/2016)

### Update

* Native iOS SDK to 8.2.1
* Native Android SDK to 8.1.1

## 8.1.0 - (01/2016)

### Update

* Native iOS SDK to 8.2.0
* Native Android SDK to 8.1.1

## 8.0.0 - (12/2015)

### API Changes

* Rebranding (SponsorPaySDK -> Fyber SDK / SP -> FYB)
* Renamed products (BrandEngage -> Rewarded Videos…)
* Introduced fluent interface API through method chaining
* Introduces Requesters
    - New API for Interstitials
    - New API for Rewarded Videos
    - New API for Offer Wall
    - New API for Starting the SDK
    - New API for Virtual Currency
*  Introduces Reporters
    - New API to report Installs
    - New API to report Rewarded Actions

### Feature Removal

* Use of multiple AppIds
* Use of credentials token
* Overriding of the currency name through SDK’s API
* adapters.info and adapter.config for mediation


### New

* New sample app shipped separately from the Plugin
* Control over precaching functionality (delay the start) (only for Fyber Videos)
* New Fyber Editor Settings menu offering:
  * Android manifest merging capabilities
  * Android mediation adapters configuration


The Fyber Unity plugin lets you access features provided by the native Fyber SDK from your Unity application.

The following features are available for both platforms, iOS and Android:

* Start the SDK session
* Launch the OfferWall
* Communicate with the VCS to get the Delta of Coins
	* Support for Multiple Currencies
* Report Rewarded Action completion
* Request and display BrandEngage offers
* Request and display Interstitial Ads
* Customer segments
* Ad placements
