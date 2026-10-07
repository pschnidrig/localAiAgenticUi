using System.ComponentModel;

namespace LocalAgenticUi.Tools;

public sealed record Note(string Title, string Content, DateTimeOffset CreatedAt);

// Holds the notes of one circuit (browser tab). The agent can only add a note
// after the user approved the call in the UI.
public sealed class NoteStore
{
    public const string Name = "save_note";

    private readonly List<Note> notes = [];

    public IReadOnlyList<Note> Notes => notes;

    public event Action? Changed;

    [Description("Saves a note for the user.")]
    public string SaveNote(
        [Description("A short title")] string title,
        [Description("The note text")] string content)
    {
        notes.Add(new Note(title, content, DateTimeOffset.Now));
        Changed?.Invoke();
        return $"Saved note '{title}'.";
    }
}
