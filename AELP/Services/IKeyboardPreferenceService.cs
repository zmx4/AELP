namespace AELP.Services;

public interface IKeyboardPreferenceService
{
    public void SetKeyboardPreference(string keyboardId);
    public string GetKeyboardPreference();
}