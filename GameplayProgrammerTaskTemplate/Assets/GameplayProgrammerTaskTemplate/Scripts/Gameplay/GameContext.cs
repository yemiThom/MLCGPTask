
/// <summary>
/// Shared context used for containing managers and systems that need to be frequently passed around.
/// </summary>
public class GameContext : SharedContext
{
    public UIManager UIManager;
    
    public GameContext()
    {
        UIManager = new UIManager("UI/UIScreens");
    }
}
