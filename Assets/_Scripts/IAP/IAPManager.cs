using UnityEngine;
using System.Collections;
using Prime31;
using System.Collections.Generic;
using System.Linq;
using AvoEx;
using System;

public class IAPManager : MonoBehaviour
{

    #if UNITY_ANDROID

    byte[] customVector = AesEncryptor.GenerateIV();

    private string _payload;

    void Awake()
    {
        var key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAndC4gVVVTc7gsQKutCYPXol7rRuZlUVCjQIbUTYnFB25/KZnJt1QqSdb+SL2kcDy4pTzfR5ubTw91u2M3YAChY/G55JQzcHzp4CW7aYjKu5nZFpn4aGwsT1QwP7lzigC7zxEY+R26T6yF2Qjm3cXJz2YrDMKVgNBL5nNDDvDuyDYIIyriT0Obe0WBckjgG/jgUfGopHIVGCLvN1hwZFLIdTZ2bseYzjT4HEQoqIQS0wx66ABC6YfeJMhQmne3eiMvOlxy2vGIhRkp0cvFEWoTJ4kLnZpwcg+5H7pEM4giUMKO+dPcDzAwx1yueIfazQdKjKBPsuOP0SpPvyJ6GdvlwIDAQAB";
        GoogleIAB.init(key);
    }

    public void PurchaseProduct(string productID)
    {
        Debug.Log("purchase " + productID);
        _payload = AesEncryptor.EncryptIV(Constants.Sec_ID, customVector);
        Debug.Log(string.Equals(productID, Constants.XWing_Product_ID));
        Debug.Log(string.Equals(productID, Constants.NoAds_Product_ID));
        Debug.Log(!PlayerProfile.Instance.CheckIfAdsAreRemoved());
        if (string.Equals(productID, Constants.XWing_Product_ID) && !PlayerProfile.Instance.CheckIfCarUnlocked(CarTypeEnum.XWingPrime))
        {
            
            GoogleIAB.purchaseProduct(productID, _payload);
        }
        else if (string.Equals(productID, Constants.NoAds_Product_ID) && !PlayerProfile.Instance.CheckIfAdsAreRemoved())
        {
            GoogleIAB.purchaseProduct(productID, _payload);
        }
    }
    // Use this for initialization
    void OnEnable()
    {
        // Listen to all events for illustration purposes
        GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
        GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
        GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
        GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
        GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
        GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
        GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;
        
    }

    void OnDisable()
    {
        // Remove all event handlers
        GoogleIABManager.billingSupportedEvent -= billingSupportedEvent;
        GoogleIABManager.billingNotSupportedEvent -= billingNotSupportedEvent;
        GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
        GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
        GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= purchaseCompleteAwaitingVerificationEvent;
        GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededEvent;
        GoogleIABManager.purchaseFailedEvent -= purchaseFailedEvent;
    }

    void billingSupportedEvent()
    {
        Debug.Log("billingSupportedEvent");
        var skus = new [] { Constants.XWing_Product_ID, Constants.NoAds_Product_ID };
        GoogleIAB.queryInventory(skus);
    }


    void billingNotSupportedEvent(string error)
    {
        Debug.Log("billingNotSupportedEvent: " + error);
    }


    void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
    {
        Debug.Log(string.Format("queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count));
        if (purchases.Any(x => x.productId.Contains("com.lastchance.xwing")))
        {
            Debug.Log("unlockcar");
            PlayerProfile.Instance.UnlockCar(CarTypeEnum.XWingPrime);
        }
        else if (purchases.Any(x => x.productId.Contains("com.lastchance.noads")))
        {
            Debug.Log("no ads");
            PlayerProfile.Instance.RemoveAds();
        }
    }


    void queryInventoryFailedEvent(string error)
    {
        Debug.Log("queryInventoryFailedEvent: " + error);
    }


    void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
    {
        Debug.Log("purchaseCompleteAwaitingVerificationEvent. purchaseData: " + purchaseData + ", signature: " + signature);
    }


    void purchaseSucceededEvent(GooglePurchase purchase)
    {
        Debug.Log("purchaseSucceededEvent: " + purchase);
        string decryptValue = AesEncryptor.DecryptIV(purchase.developerPayload, customVector);
        Debug.Log(decryptValue + purchase.productId);
        if (decryptValue == Constants.Sec_ID)
        {
            if (purchase.productId == Constants.XWing_Product_ID)
                PlayerProfile.Instance.UnlockCar(CarTypeEnum.XWingPrime);
            else if (purchase.productId == Constants.NoAds_Product_ID)
                PlayerProfile.Instance.RemoveAds();
        }
        
    }


    void purchaseFailedEvent(string error, int response)
    {
        Debug.Log("purchaseFailedEvent: " + error + ", response: " + response);
    }
    #endif
}
