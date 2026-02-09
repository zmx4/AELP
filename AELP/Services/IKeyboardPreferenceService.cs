namespace AELP.Services;

public interface IKeyboardPreferenceService
{
    string GetChoiceOptionKeys();
    void SetChoiceOptionKeys(string keys);
}