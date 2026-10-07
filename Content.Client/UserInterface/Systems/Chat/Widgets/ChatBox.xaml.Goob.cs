using System.Linq;
using Content.Shared.Chat;

namespace Content.Client.UserInterface.Systems.Chat.Widgets;

public partial class ChatBox
{
    private void UpdateCoalescence(bool value)
    {
        _coalescence = value;
        Repopulate();

        foreach (var child in Contents.Children.ToArray())
        {
            if (child.Name != "_v_scroll")
            {
                Contents.RemoveChild(child);
            }
        }
    }

        public void Repopulate()
        {
            Contents.Clear();

            // Goobstation start
            foreach (var child in Contents.Children.ToArray())
            {
                if (child.Name != "_v_scroll")
                {
                    Contents.RemoveChild(child);
                }
            }
            // Goobstation end

            // F14: the panel is now empty, so drop the coalescing state too. Otherwise
            // F14: the first replayed message can match the stale _lastLine, take the
            // F14: coalesce branch and call RemoveEntry(^2) with only one entry present.
            _lastLine = null;
            _lastLineRepeatCount = 0;

            foreach (var message in _controller.History)
            {
                OnMessageAdded(message.Item2);
            }
        }

        private void OnChannelFilter(ChatChannel channel, bool active)
        {
            Contents.Clear();

            // Goobstation start
            foreach (var child in Contents.Children.ToArray())
            {
                if (child.Name != "_v_scroll")
                {
                    Contents.RemoveChild(child);
                }
            }
            // Goobstation end

            // F14: the panel is now empty, so drop the coalescing state too. Otherwise
            // F14: the first replayed message can match the stale _lastLine, take the
            // F14: coalesce branch and call RemoveEntry(^2) with only one entry present.
            _lastLine = null;
            _lastLineRepeatCount = 0;

            foreach (var message in _controller.History)
            {
                OnMessageAdded(message.Item2);
            }

            if (active)
            {
                _controller.ClearUnfilteredUnreads(channel);
            }
        }
}
