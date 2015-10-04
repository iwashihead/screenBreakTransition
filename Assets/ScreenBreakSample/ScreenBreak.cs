using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// スクリーンが割れるようなトランジションを表示.
/// </summary>
public class ScreenBreak : MonoBehaviour {
	[SerializeField] private Animation breakAnim;
	[SerializeField] private List<Camera> cameras;
	[SerializeField] private float scale = 1f;
	[SerializeField] private RenderTexture renderTex;
	[SerializeField] private Material renderMaterial;
	[SerializeField] private Camera targetCamera;

	private bool _playFlag;
	private bool _targetCamEnabled;


	void Awake () {
		if (renderTex == null) {
			renderTex = new RenderTexture(
				(int)(Screen.width / scale),
				(int)(Screen.height / scale), 24);
			renderTex.enableRandomWrite = false;
		}
		_playFlag = false;
		gameObject.SetActive(false);
	}

	/// <summary>
	/// スクリーン破壊演出の開始.
	/// </summary>
	[ContextMenu("Play")]
	public void Play()
	{
		if (_playFlag) return;

		_playFlag = true;

		gameObject.SetActive(true);
		StartCoroutine(co_Play());
	}


	IEnumerator co_Play()
	{
		if (_playFlag == false) yield break;

		_targetCamEnabled = targetCamera.enabled;
		targetCamera.enabled = true;

		// カメラにレンダラテクスチャをセット.
		foreach (var cam in cameras)
			cam.targetTexture = renderTex;
		
		renderMaterial.mainTexture = renderTex;
		breakAnim.Play("breaks");


		// 1フレ待機.
		yield return null;


		// カメラにレンダラテクスチャをリセット.
		foreach (var cam in cameras)
			cam.targetTexture = null;


		// アニメーション終わるまで待機
		yield return new WaitForSeconds(breakAnim["breaks"].length);


		// 後処理
		_playFlag = false;
		gameObject.SetActive(false);
		targetCamera.enabled = _targetCamEnabled;
	}
}
