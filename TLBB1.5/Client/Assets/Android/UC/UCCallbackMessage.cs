using UnityEngine;
using System.Collections;
using System;
using Games.CharacterLogic;
/// <summary>
/// 接收 SDK 的回调消息，进行处理。
/// 该脚本应关联到每一个场景的 "Main Camera" 对象，以能接收SDK回调的消息。
/// 下面各方法中的逻辑处理，在游戏中应修改为真实的逻辑。
/// </summary>
public class UCCallbackMessage : MonoBehaviour
{
#if UNITY_ANDROID
	public void OnCallResult(string jsonstr){
		Debug.Log("OnCallResult - OnCallResult message: jsonstr=" + jsonstr);
		JsonData jsonobj = JsonMapper.ToObject (jsonstr);
		string func = (string)jsonobj ["func"];
		Debug.Log("----OnCallResult1 func----"+func);
		string json = (string)jsonobj ["json"];
		Debug.Log("----OnCallResult2 json----"+json);
		if(func.Equals("onLogin")){
			AndroidConfig.SetThirdLoginInfo(json);
			
			GameObject accountsUI = AccountManager.Instance.GetAccountsUI();
			if(accountsUI != null)
			{
				if(!accountsUI.gameObject.activeSelf)
					accountsUI.SetActive(true);
					accountsUI.GetComponent<CyouAccounts>().SwitchRefresh();
					if(AndroidConfig.is360Channel()){
						accountsUI.GetComponent<CyouAccounts>().infoTime = 0f;
					}
			}
			Debug.Log("----OnCallResult3  onLogin ----"+json);
		}else if(func.Equals("onPay")){
			onPay(json);
		}else if(func.Equals("onLogout")){
			Debug.Log("----OnCallResult4  onLogout ----"+json);
			OnLogout(json);
		}
		
	}
	private void onPay (string jsonOrder)
	{
		Debug.Log("----onPay  ----"+jsonOrder);
		JsonData jsonobj = JsonMapper.ToObject (jsonOrder);
		string orderId = (string)jsonobj["orderId"];

        Debug.Log("pllog_onPay_SendVarify :" + jsonOrder);
		//客户单直接发送消息
		PurchaseHelper.Instance().VarifyJavaOrder(	(string)jsonobj["gid"], 
													(string)jsonobj["pid"], 
													(string)jsonobj["goodsPrice"], 
													(string)jsonobj["orderId"]);

		
		GlobalSave.SetCyouStoreLossGoodInfoTempToReal(orderId, Obj_MyselfPlayer.GetMe().accountID); // 将保存的临时信息转成正式信息
		//游戏根据需要进行订单处理，一般需要把订单号传回游戏服务器，在服务器上保存
		//PurchaseHelper.Instance().UCAppPayResult(0, "");

	}
	
	//UC代码重构 java端注销成功才会调用此方法-用于UC游戏中注销 lihao_yd 2013-12-09
	private void OnLogout (string msg)
	{
		Debug.Log("----UNITY-Logout-msg=="+msg);
		//输出退出登录结果到页面(接入后删除)
		//SendMessage(string.Format ("UCCallbackMessage - OnLogout: code={0}, msg={1}", code, msg));
		
		//当前登录用户已退出，应将游戏切换到未登录的状态。

		//UCConfig.logined = false;
		//清空登录数据
		if(AndroidConfig.isLogin())
		{
			AndroidConfig.SetThirdLoginInfo("");
			Debug.Log("----UNITY-Logout-清空登录数据1");
		}
		if(!GameManager.Instance.sceneName.Equals(Utils.UI_NAME_Login))
	    {
			Debug.Log("----UNITY-Logout-清空登录数据2");	
			OnReturnToLogin();
		}
		// 刷新账号相关UI //
		GameObject accountsUI = AccountManager.Instance.GetAccountsUI();
		if(accountsUI != null)
		{
			if(!accountsUI.gameObject.activeSelf)
					accountsUI.SetActive(true);
			accountsUI.GetComponent<CyouAccounts>().Refresh();//只刷新顶部的账号UI
		}			
	}
	
	
#endif
	//王明磊 返回登录
	public void OnReturnToLogin() {
		/*  原返回登录部分代码
//		GameManager.Instance.LoadLevel(Utils.UI_NAME_Login);
		//清空session id
		card.net.HTTPClientAPI.cleanSessionId();
		//reset login当前菜单为splahcontroller
		LoginLogic.needResetLogin = true;
		MainUILogic.needResetLogin = true;
		//清除临时切换数据
		//update
		//Obj_MyselfPlayer.GetMe().updateHeroItem = null;
		//Obj_MyselfPlayer.GetMe().updateMaterialItems = new UserCardItem[6];
		//evolution
		Obj_MyselfPlayer.GetMe().evolutionHeroItem = null;
		Obj_MyselfPlayer.GetMe().evolutionMaterialItems = new Games.LogicObject.UserCardItem[5];
		//strengthen
		Obj_MyselfPlayer.GetMe().strengthenHeroItem = null;
		
		Obj_MyselfPlayer.ReleaseMe();
		
		//清空新手引导状态
		GuideManager.Instance.guideTimeOut();
		  //NetworkSender.Instance().Login(LoginDone, AccountManager.Instance.GetLoginAccountID());
		//回到主菜单
		GameManager.Instance.LoadLevel(Utils.UI_NAME_Login);
#if UNITY_ANDROID
		//AndroidConfig.directShowLoginUI();
#endif
		*/
		
		//新的返回登录 2014-01-18
		//GameManager.Instance.LoadLevel(Utils.UI_NAME_Login);
		//清空session id
		card.net.HTTPClientAPI.cleanSessionId();
		//reset login当前菜单为splahcontroller
		LoginLogic.needResetLogin = true;
		MainUILogic.needResetLogin = true;
		//清除临时切换数据
		//update
		//Obj_MyselfPlayer.GetMe().updateHeroItem = null;
		//Obj_MyselfPlayer.GetMe().updateMaterialItems = new UserCardItem[6];
		//evolution
		Obj_MyselfPlayer.GetMe().evolutionHeroItem = null;
		Obj_MyselfPlayer.GetMe().evolutionMaterialItems = new Games.LogicObject.UserCardItem[5];
		//strengthen
		Obj_MyselfPlayer.GetMe().strengthenHeroItem = null;
		
		Obj_MyselfPlayer.ReleaseMe();
		//清空按钮闪烁状态
        MainController.needFlashLunJian = false;
        MainController.needFlashWulin = false;
		//清空新手引导状态
		GuideManager.Instance.guideTimeOut();
		  //NetworkSender.Instance().Login(LoginDone, AccountManager.Instance.GetLoginAccountID());
		//回到主菜单
		if (AccountManager.userType != AccountManager.UserType.OldUser)
			PlayerPrefs.SetInt("InGameBackLogin",1); //标记玩家是否在游戏中返回登录
		GameManager.Instance.LoadLevel(Utils.UI_NAME_Login);
	}
}