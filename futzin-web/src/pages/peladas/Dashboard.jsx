import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { peladasAPI } from '../../services/api';
import Button from '../../components/common/Button';
import Card from '../../components/common/Card';
import './Dashboard.css';

const Dashboard = () => {
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const [peladas, setPeladas] = useState([]);
  const [myPeladas, setMyPeladas] = useState([]);
  const [myParticipations, setMyParticipations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('all'); // all, my-peladas, my-participations
  const [menuOpen, setMenuOpen] = useState(false);

  useEffect(() => {
    fetchData();
  }, []);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (menuOpen && !event.target.closest('.menu-container')) {
        setMenuOpen(false);
      }
    };

    document.addEventListener('click', handleClickOutside);
    return () => document.removeEventListener('click', handleClickOutside);
  }, [menuOpen]);

  const fetchData = async () => {
    try {
      const [allResponse, myResponse, participationsResponse] = await Promise.all([
        peladasAPI.getUpcoming(),
        peladasAPI.getMyPeladas(),
        peladasAPI.getMyParticipations(),
      ]);

      setPeladas(allResponse.data);
      setMyPeladas(myResponse.data);
      setMyParticipations(participationsResponse.data);
    } catch (error) {
      console.error('Erro ao carregar peladas:', error);
    } finally {
      setLoading(false);
    }
  };

  const getDisplayPeladas = () => {
    switch (activeTab) {
      case 'my-peladas':
        return myPeladas;
      case 'my-participations':
        return myParticipations;
      default:
        return peladas;
    }
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('pt-BR', {
      dateStyle: 'short',
      timeStyle: 'short',
    }).format(date);
  };

  const getFieldTypeLabel = (fieldType) => {
    const types = {
      0: 'Campo',
      1: 'Quadra',
      2: 'Society',
      3: 'Terrão',
    };
    return types[fieldType] || 'Campo';
  };

  if (loading) {
    return <div className="dashboard-loading">Carregando...</div>;
  }

  return (
    <div className="dashboard">
      <header className="dashboard-header">
        <div className="dashboard-header-content">
          <div className="header-logo">
            <img src="/logo-sem-nome.png" alt="Futzin" className="header-logo-full" />
          </div>
          <div className="header-center">
            <span className="user-name">Olá, {user?.name}!</span>
          </div>
          <div className="dashboard-header-actions">
            <div className="menu-container">
              <button className="menu-button" onClick={() => setMenuOpen(!menuOpen)}>
                ⋮
              </button>
              {menuOpen && (
                <div className="menu-dropdown">
                  <button className="menu-item" onClick={() => { logout(); setMenuOpen(false); }}>
                    Sair
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      </header>

      <main className="dashboard-main">
        <div className="dashboard-container">
          <div className="dashboard-top">
            <h2>Peladas</h2>
            <Button onClick={() => navigate('/peladas/new')}>+ Nova Pelada</Button>
          </div>

          <div className="dashboard-tabs">
            <button
              className={`tab ${activeTab === 'all' ? 'tab-active' : ''}`}
              onClick={() => setActiveTab('all')}
            >
              Todas ({peladas.length})
            </button>
            <button
              className={`tab ${activeTab === 'my-peladas' ? 'tab-active' : ''}`}
              onClick={() => setActiveTab('my-peladas')}
            >
              Minhas Peladas ({myPeladas.length})
            </button>
            <button
              className={`tab ${activeTab === 'my-participations' ? 'tab-active' : ''}`}
              onClick={() => setActiveTab('my-participations')}
            >
              Participando ({myParticipations.length})
            </button>
          </div>

          <div className="peladas-grid">
            {getDisplayPeladas().length === 0 ? (
              <div className="empty-state">
                <p>Nenhuma pelada encontrada</p>
                {activeTab === 'all' && (
                  <Button onClick={() => navigate('/peladas/new')}>Criar primeira pelada</Button>
                )}
              </div>
            ) : (
              getDisplayPeladas().map((pelada) => (
                <Card
                  key={pelada.id}
                  className="pelada-card"
                  onClick={() => navigate(`/peladas/${pelada.id}`)}
                >
                  <div className="pelada-card-header">
                    <h3>{pelada.name}</h3>
                    <span className={`badge ${pelada.isPublic ? 'badge-success' : 'badge-warning'}`}>
                      {pelada.isPublic ? 'Pública' : 'Privada'}
                    </span>
                  </div>

                  <div className="pelada-card-info">
                    <div className="info-item">
                      <span className="info-label">📅 Data:</span>
                      <span>{formatDate(pelada.date)}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">📍 Local:</span>
                      <span>{pelada.location}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">⚽ Tipo:</span>
                      <span>{getFieldTypeLabel(pelada.fieldType)}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">💰 Preço:</span>
                      <span>R$ {pelada.price.toFixed(2)}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">👥 Vagas:</span>
                      <span>
                        {pelada.currentPlayers}/{pelada.maxPlayers}
                      </span>
                    </div>
                  </div>

                  <div className="pelada-card-creator">
                    Criado por: {pelada.createdByName}
                  </div>
                </Card>
              ))
            )}
          </div>
        </div>
      </main>
    </div>
  );
};

export default Dashboard;
