using UnityEngine;

public class microphone : MonoBehaviour
{
	public GameObject objectToToggle;
	public bool muted;
	public bool isForceMuted;

	private bool mutedBeforeForce;

	private void Awake()
	{
		SyncMutedState();
	}

	private void OnMouseDown()
	{
		if (isForceMuted)
			return;

		ToggleObject();
	}

	public void ToggleObject()
	{
		if (isForceMuted)
			return;

		if (objectToToggle == null)
		{
			muted = false;
			return;
		}

		objectToToggle.SetActive(!objectToToggle.activeSelf);
		SyncMutedState();
	}

	public void ForceMute()
	{
		if (!isForceMuted)
			mutedBeforeForce = muted;

		isForceMuted = true;
		muted = true;

		if (objectToToggle != null)
			objectToToggle.SetActive(true);
	}

	public void ForceUnmute()
	{
		if (!isForceMuted)
			return;

		isForceMuted = false;
		muted = mutedBeforeForce;

		if (objectToToggle != null)
			objectToToggle.SetActive(muted);
	}
	private void SyncMutedState()
	{
		muted = objectToToggle != null && objectToToggle.activeSelf;
	}
}
