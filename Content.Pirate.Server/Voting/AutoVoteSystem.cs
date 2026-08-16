using Content.Server.GameTicking;
using Content.Server.Voting.Managers;
using Content.Pirate.Common.Voting;
using Content.Shared.GameTicking;
using Content.Shared.Voting;
using Robust.Server.Player;
using Robust.Shared.Configuration;

namespace Content.Pirate.Server.Voting
{
    public sealed class AutoVoteSystem : EntitySystem
    {
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        [Dependency] private readonly IVoteManager _voteManager = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;

        private bool _shouldVoteNextJoin = false;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<LobbyReadyUpEvent>(OnLobbyReadyUp);
            SubscribeLocalEvent<PlayerJoinedLobbyEvent>(OnPlayerJoinedLobby);
        }

        private void OnLobbyReadyUp(LobbyReadyUpEvent ev)
        {
            if (_playerManager.PlayerCount == 0)
            {
                _shouldVoteNextJoin = true;
                return;
            }

            // F14: GameTicker guards raising LobbyReadyUpEvent with #if !DEBUG, but that guard
            // F14: is missed in the DebugOpt config CI builds with. Auto-votes started here push
            // F14: net messages to the dummy sessions integration tests create, which throw.
#if !DEBUG
            _voteManager.CreateStandardVote(null, StandardVoteType.Map);
            _voteManager.CreateStandardVote(null, StandardVoteType.Preset);
#endif
        }

        private void OnPlayerJoinedLobby(PlayerJoinedLobbyEvent ev)
        {
            if (!_shouldVoteNextJoin)
                return;

            OnLobbyReadyUp(new LobbyReadyUpEvent());
            _shouldVoteNextJoin = false;
        }
    }
}
