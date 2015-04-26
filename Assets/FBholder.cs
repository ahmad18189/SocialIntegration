using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class FBholder : MonoBehaviour {
	public GameObject UIFBIsLogedIN;
	public GameObject UIFBIsNotLogedIN;
	public GameObject UIFBAvatar;
	public GameObject UIFBUserName;
	public Text ScoreDebug ; 

	private Dictionary<string,string> profile  = null;
	private List<object> scoresList = null;

	/// <summary>
	/// When Game Loades 
	/// -I set Loggedin GO active to FAlse 
	/// -i Init FB 
	/// </summary>
	void Awake(){
		UIFBIsLogedIN.SetActive(false);
		FB.Init (Setinit, onHideUnit);
	}

	/// <summary>
	/// This A Call Back for FB Done 
	/// -FB is Done 
	/// --Manage UI Element Check if loged in 
	/// </summary>
	private void Setinit(){
		Debug.Log ("FB Init Done");
		if (FB.IsLoggedIn) {
			MangeUILoginActions(true);
			Debug.Log("FB isloged in ");
		} else {
			MangeUILoginActions(false);

		}
	}

	/// <summary>
	/// in Case Game Susspended we sop the game time scale 
	/// </summary>
	/// <param name="isGameShown">If set to <c>true</c> the game shown.</param>
	private void onHideUnit(bool isGameShown)
	{
		if (!isGameShown) {
			Time.timeScale =0;
		} else {
			Time.timeScale= 1;
		}
	}
	#region FaceBook Main Functions 
	/// <summary>
	/// FB login Function 
	///-Ask for Emal Permations 
	/// </summary>
	public void FBlogin()
	{
		FB.Login ("email", AuthCallBack); 
	}
	 /// <summary>
	 ///A Call Back Function for login 
	/// --Check for login sucsses 
	/// </summary>
	 /// <param name="result">Result.</param>
	void AuthCallBack(FBResult result)
	{
		if (FB.IsLoggedIn) {
			Debug.Log ("FB login Successed");
			MangeUILoginActions(true);
		} else {
			Debug.Log("FB Login Failed ");
			MangeUILoginActions(false);
		}
	}
	/// <summary>
	/// Function For manageing UI elment 
	/// -Case Of loged in 
	/// -Case is Not 
	/// 
	/// </summary>
	/// <param name="IsLoggedIn">If set to <c>true</c> is logged in.</param>
	private void MangeUILoginActions(bool IsLoggedIn)
	{
		if (IsLoggedIn) {
			UIFBIsLogedIN.SetActive(true);
			UIFBIsNotLogedIN.SetActive(false);

			//Get Avatar
			FB.API(Util.GetPictureURL("me",60,60),Facebook.HttpMethod.GET,DealWithProfilPic);

			//Get NAme
			FB.API("/me?fields=id,first_name",Facebook.HttpMethod.GET,DealWithUserName);

			
		} else {
			UIFBIsLogedIN.SetActive(false);
			UIFBIsNotLogedIN.SetActive(true);
		}

	}
	/// <summary>
	/// Call Back Functuion for calling FB user Profile Pic 
	/// </summary>
	/// <param name="result">Result.</param>
	void DealWithProfilPic(FBResult result)
	{
		if (result.Error != null) {
			Debug.Log("Problem with profil pic");
			return;
		}
		Image UserAvatar = UIFBAvatar.GetComponent<Image> ();
		UserAvatar.sprite = Sprite.Create (result.Texture, new Rect (0, 0, 60, 60), Vector2.zero);

	}
	/// <summary>
	/// Call Back Functuion for calling FB user Name
	/// </summary>
	/// <param name="result">Result.</param>
	void DealWithUserName(FBResult result){
		if (result.Error != null) {
			Debug.Log("Problem with profil user name");
			return;
		}
		profile = Util.DeserializeJSONProfile (result.Text);
		Text UserName = UIFBUserName.GetComponent<Text> ();
		UserName.text = "Hello , " + profile["first_name"];
		Debug.Log("THE END");
	}
	/// <summary>
	/// Share With Friends Button Action 
	/// </summary>
	public void ShareWithFriends()
	{
		FB.Feed(linkCaption:"Test For TEST ",linkDescription:"Test For Ever ",
		        picture:"http://farm5.static.flickr.com/4070/4444826388_2cd74d322d_m.jpg",link:"Mad Test !!");
	}
	/// <summary>
	/// Invites the friends.
	/// </summary>
	public void InviteFriends()
	{
		FB.AppRequest (
			message: "Test invite Message Dude !!",
			title: "Test Invite "
		);
	}

	public void QueryScore ()
	{ScoreDebug.text = "";
		FB.API ("/app/scores?fields=score,user.limit(30)", Facebook.HttpMethod.GET, ScoreCallBack);
		Debug.Log("THE END");
	}
	private void ScoreCallBack(FBResult result )  { 
		scoresList = Util.DeserializeScores (result.Text);
		foreach (object score in scoresList) {
			var entry=(Dictionary<string,object>)score;
			var user = (Dictionary<string,object>)entry["user"];
			ScoreDebug.text += "UN : "+user["name"]+"\n"+ " Score : "+ entry["score"]+"\n\n";
		}
	} 

	public void SetScore()
	{
		var scoreData = new Dictionary<string,string> ();
		scoreData ["score"] = Random.Range (100, 2300).ToString(); 
		FB.API ("/me/scores", Facebook.HttpMethod.POST, delegate(FBResult result) {
			Debug.Log ("Score Submmeited " + result.Text);
		},scoreData);
	}




	#endregion

}
