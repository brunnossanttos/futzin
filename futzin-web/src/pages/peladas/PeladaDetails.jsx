import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { peladasAPI, teamsAPI, invitesAPI } from '../../services/api';
import Button from '../../components/common/Button';
import Card from '../../components/common/Card';
import Input from '../../components/common/Input';
import './PeladaDetails.css';

const PeladaDetails = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { user } = useAuth();

  const [pelada, setPelada] = useState(null);
  const [teams, setTeams] = useState([]);
  const [inviteEmail, setInviteEmail] = useState('');
  const [inviteLink, setInviteLink] = useState('');
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  useEffect(() => {
    fetchData();
  }, [id]);

  const fetchData = async () => {
    try {
      const [peladaResponse, teamsResponse] = await Promise.all([
        peladasAPI.getDetails(id),
        teamsAPI.getByPelada(id),
      ]);

      setPelada(peladaResponse.data);
      setTeams(teamsResponse.data);

      if (peladaResponse.data.createdById === user.id) {
        setInviteLink(`${window.location.origin}/join/${peladaResponse.data.inviteToken || 'token'}`);
      }
    } catch (error) {
      console.error('Erro ao carregar pelada:', error);
      showMessage('error', 'Erro ao carregar dados da pelada');
    } finally {
      setLoading(false);
    }
  };

  const showMessage = (type, text) => {
    setMessage({ type, text });
    setTimeout(() => setMessage({ type: '', text: '' }), 5000);
  };

  const handleJoin = async () => {
    setActionLoading(true);
    try {
      await peladasAPI.join(id);
      showMessage('success', 'Você entrou na pelada com sucesso!');
      fetchData();
    } catch (error) {
      showMessage('error', error.response?.data?.message || 'Erro ao entrar na pelada');
    } finally {
      setActionLoading(false);
    }
  };

  const handleLeave = async () => {
    if (!confirm('Tem certeza que deseja sair desta pelada?')) return;

    setActionLoading(true);
    try {
      await peladasAPI.leave(id);
      showMessage('success', 'Você saiu da pelada');
      fetchData();
    } catch (error) {
      showMessage('error', error.response?.data?.message || 'Erro ao sair da pelada');
    } finally {
      setActionLoading(false);
    }
  };

  const handleGenerateTeams = async () => {
    setActionLoading(true);
    try {
      const response = await teamsAPI.generate(id);
      setTeams(response.data);
      showMessage('success', 'Times sorteados com sucesso!');
    } catch (error) {
      showMessage('error', error.response?.data?.message || 'Erro ao sortear times');
    } finally {
      setActionLoading(false);
    }
  };

  const handleInviteUser = async (e) => {
    e.preventDefault();
    if (!inviteEmail.trim()) return;

    setActionLoading(true);
    try {
      await invitesAPI.invite(id, inviteEmail);
      showMessage('success', `Convite enviado para ${inviteEmail}`);
      setInviteEmail('');
      fetchData();
    } catch (error) {
      showMessage('error', error.response?.data?.message || 'Erro ao enviar convite');
    } finally {
      setActionLoading(false);
    }
  };

  const handleRemoveInvite = async (inviteId) => {
    if (!confirm('Remover este convite?')) return;

    try {
      await invitesAPI.remove(id, inviteId);
      showMessage('success', 'Convite removido');
      fetchData();
    } catch (error) {
      showMessage('error', 'Erro ao remover convite');
    }
  };

  const copyInviteLink = () => {
    navigator.clipboard.writeText(inviteLink);
    showMessage('success', 'Link copiado para a área de transferência!');
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('pt-BR', {
      dateStyle: 'full',
      timeStyle: 'short',
    }).format(date);
  };

  const getFieldTypeLabel = (fieldType) => {
    const types = { 0: 'Campo', 1: 'Quadra', 2: 'Society', 3: 'Terrão' };
    return types[fieldType] || 'Campo';
  };

  const isCreator = pelada?.createdById === user.id;
  const isParticipating = pelada?.participants?.some((p) => p.userId === user.id);

  if (loading) {
    return <div className="details-loading">Carregando...</div>;
  }

  if (!pelada) {
    return <div className="details-error">Pelada não encontrada</div>;
  }

  return (
    <div className="pelada-details">
      <div className="details-container">
        <Button size="small" onClick={() => navigate('/dashboard')}>
          ← Voltar
        </Button>

        {message.text && (
          <div className={`message message-${message.type}`}>{message.text}</div>
        )}

        <Card className="details-main">
          <div className="details-header">
            <div>
              <h1>{pelada.name}</h1>
              <span className={`badge ${pelada.isPublic ? 'badge-success' : 'badge-warning'}`}>
                {pelada.isPublic ? 'Pública' : 'Privada'}
              </span>
            </div>
            {isCreator && (
              <Button size="small" onClick={() => navigate(`/peladas/${id}/edit`)}>
                Editar
              </Button>
            )}
          </div>

          {pelada.description && <p className="details-description">{pelada.description}</p>}

          <div className="details-info-grid">
            <div className="info-box">
              <span className="info-icon">📅</span>
              <div>
                <div className="info-label">Data e Hora</div>
                <div className="info-value">{formatDate(pelada.date)}</div>
              </div>
            </div>

            <div className="info-box">
              <span className="info-icon">📍</span>
              <div>
                <div className="info-label">Local</div>
                <div className="info-value">{pelada.location}</div>
              </div>
            </div>

            <div className="info-box">
              <span className="info-icon">⚽</span>
              <div>
                <div className="info-label">Tipo</div>
                <div className="info-value">{getFieldTypeLabel(pelada.fieldType)}</div>
              </div>
            </div>

            <div className="info-box">
              <span className="info-icon">💰</span>
              <div>
                <div className="info-label">Preço</div>
                <div className="info-value">R$ {pelada.price.toFixed(2)}</div>
              </div>
            </div>

            <div className="info-box">
              <span className="info-icon">👥</span>
              <div>
                <div className="info-label">Vagas</div>
                <div className="info-value">
                  {pelada.currentPlayers}/{pelada.maxPlayers}
                </div>
              </div>
            </div>

            <div className="info-box">
              <span className="info-icon">👤</span>
              <div>
                <div className="info-label">Organizador</div>
                <div className="info-value">{pelada.createdByName}</div>
              </div>
            </div>
          </div>

          {!isCreator && !isParticipating && !pelada.isFull && (
            <Button fullWidth onClick={handleJoin} loading={actionLoading}>
              Entrar na Pelada
            </Button>
          )}

          {isParticipating && !isCreator && (
            <Button fullWidth variant="danger" onClick={handleLeave} loading={actionLoading}>
              Sair da Pelada
            </Button>
          )}
        </Card>

        {isCreator && !pelada.isPublic && (
          <Card title="🔗 Link de Convite">
            <p>Compartilhe este link para convidar pessoas:</p>
            <div className="invite-link-box">
              <input type="text" value={inviteLink} readOnly className="invite-link-input" />
              <Button size="small" onClick={copyInviteLink}>
                Copiar
              </Button>
            </div>

            <form onSubmit={handleInviteUser} className="invite-form">
              <Input
                placeholder="email@exemplo.com"
                type="email"
                value={inviteEmail}
                onChange={(e) => setInviteEmail(e.target.value)}
              />
              <Button type="submit" loading={actionLoading}>
                Convidar
              </Button>
            </form>

            {pelada.invites && pelada.invites.length > 0 && (
              <div className="invites-list">
                <h4>Convites Enviados:</h4>
                {pelada.invites.map((invite) => (
                  <div key={invite.id} className="invite-item">
                    <div>
                      <strong>{invite.userName}</strong>
                      <span className="invite-email">{invite.userEmail}</span>
                      {invite.isAccepted && <span className="invite-accepted">✓ Aceito</span>}
                    </div>
                    <Button
                      size="small"
                      variant="danger"
                      onClick={() => handleRemoveInvite(invite.id)}
                    >
                      Remover
                    </Button>
                  </div>
                ))}
              </div>
            )}
          </Card>
        )}

        <Card title="👥 Participantes">
          {pelada.participants && pelada.participants.length > 0 ? (
            <div className="participants-list">
              {pelada.participants.map((participant) => (
                <div key={participant.userId} className="participant-item">
                  <div>
                    <strong>{participant.userName}</strong>
                    {participant.hasPaid && <span className="paid-badge">💰 Pago</span>}
                    {participant.teamName && <span className="team-badge">{participant.teamName}</span>}
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p>Nenhum participante ainda</p>
          )}
        </Card>

        {isCreator && pelada.currentPlayers >= 2 && (
          <Card title="⚽ Times">
            <Button fullWidth onClick={handleGenerateTeams} loading={actionLoading}>
              {teams.length > 0 ? 'Re-sortear Times' : 'Sortear Times'}
            </Button>

            {teams.length > 0 && (
              <div className="teams-grid">
                {teams.map((team) => (
                  <div key={team.id} className="team-card" style={{ borderColor: team.color }}>
                    <h4>{team.name}</h4>
                    <div className="team-color" style={{ backgroundColor: team.color }} />
                    <p>{team.playerCount} jogadores</p>
                    <ul>
                      {team.playerNames.map((name, idx) => (
                        <li key={idx}>{name}</li>
                      ))}
                    </ul>
                  </div>
                ))}
              </div>
            )}
          </Card>
        )}
      </div>
    </div>
  );
};

export default PeladaDetails;
