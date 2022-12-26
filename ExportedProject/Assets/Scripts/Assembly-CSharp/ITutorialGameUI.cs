public interface ITutorialGameUI
{
	void SetTutorialText(string text);

	void SetTutorialUIEvent(ITutorialUIEvent tEvent);

	void EnableTutorialOKButton(bool enable);
}
