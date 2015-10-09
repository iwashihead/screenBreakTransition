using UnityEngine;
using System;
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
	/// <summary>
	/// グレースケール色変化演出を使う場合、GrayScaleParametricシェーダーをセットする必要あり
	/// </summary>
	[SerializeField] private Material renderMaterial;
	[SerializeField] private Camera targetCamera;

	/// <summary>
	/// 破壊アニメーション名.
	/// </summary>
	public string breakAnimationName = "Break";
	/// <summary>
	/// 位置リセットアニメーション名.
	/// </summary>
	public string resetAnimationName = "Reset";
	/// <summary>
	/// 破壊アニメーションの再生速度.
	/// </summary>
	public float animationSpeed = 1f;
	/// <summary>
	/// 色変化の演出時間.
	/// </summary>
	public float colorTweenTime = 1f;
	/// <summary>
	/// 色変化の後の待機時間.
	/// </summary>
	public float afterTweenWaitTime = 0.5f;
	/// <summary>
	/// 再生中かどうか.(true=再生中)
	/// </summary>
	public bool IsPlaying { get { return _playFlag; } }


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
	public void Play(bool autoCollectCamera = true, Action OnColorChangeEnd = null, Action OnAnimationEnd = null)
	{
		if (_playFlag) return;

		_playFlag = true;

		// カメラの自動取得
		if (autoCollectCamera)
		{
			cameras = new List<Camera>(FindObjectsOfType<Camera>());
		}

		// Depthを更新(最前面に表示)
		if (cameras != null)
		{
			float max_depth = 0f;
			foreach (var cam in cameras)
			{
				if (cam.depth > max_depth)
					max_depth = cam.depth;
			}
			targetCamera.depth = max_depth + 1f;
		}

		gameObject.SetActive(true);
		StartCoroutine(co_Play(OnColorChangeEnd, OnAnimationEnd));
	}


	IEnumerator co_Play(Action OnColorChangeEnd, Action OnAnimationEnd)
	{
		if (_playFlag == false) yield break;

		_targetCamEnabled = targetCamera.enabled;
		targetCamera.enabled = true;
		breakAnim.gameObject.SetActive(true);

		// カメラにレンダラテクスチャをセット.
		foreach (var cam in cameras)
			cam.targetTexture = renderTex;
		
		renderMaterial.mainTexture = renderTex;

		// 表示リセット.
		breakAnim.Play(resetAnimationName);

		// 色変化演出.
		if (colorTweenTime <= 0) {
			// コールバック.
			if (OnColorChangeEnd != null)
				OnColorChangeEnd();
		}
		else {
			float counter = 0f;
			while (counter < colorTweenTime)
			{
				yield return null;
				float t = Mathf.Clamp01(counter / colorTweenTime);
				if (renderMaterial.HasProperty("_Amount"))
					renderMaterial.SetFloat("_Amount", t);
				counter += Time.deltaTime;
			}
			// 完了.
			if (renderMaterial.HasProperty("_Amount"))
				renderMaterial.SetFloat("_Amount", 1f);
			
			// コールバック.
			if (OnColorChangeEnd != null)
				OnColorChangeEnd();
		}

		// 演出後待機.
		yield return new WaitForSeconds(afterTweenWaitTime);

		// 再生開始.
		breakAnim.Play(breakAnimationName);

		// 1フレ待機.
		yield return null;

		// カメラにレンダラテクスチャをリセット.
		foreach (var cam in cameras)
			cam.targetTexture = null;

		// アニメーション終わるまで待機
		yield return new WaitForSeconds(breakAnim[breakAnimationName].length);

		// 後処理
		_playFlag = false;
		gameObject.SetActive(false);
		targetCamera.enabled = _targetCamEnabled;

		// コールバック.
		if (OnAnimationEnd != null)
			OnAnimationEnd();
	}
}
