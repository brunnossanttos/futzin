import { useState, useEffect } from 'react';
import { userStatsService } from '../services/userStatsService';

export default function UserStatsPage({ userId }) {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadStats();
  }, [userId]);

  const loadStats = async () => {
    try {
      setLoading(true);
      const data = await userStatsService.getUserStats(userId);
      setStats(data);
    } catch (error) {
      console.error('Erro ao carregar estatísticas:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleRecalculate = async () => {
    try {
      await userStatsService.recalculate(userId);
      loadStats();
    } catch (error) {
      console.error('Erro ao recalcular estatísticas:', error);
    }
  };

  if (loading) {
    return <div className="loading">Carregando estatísticas...</div>;
  }

  if (!stats) {
    return <div className="no-stats">Nenhuma estatística disponível</div>;
  }

  return (
    <div className="user-stats-page">
      <div className="stats-header">
        <h1>Minhas Estatísticas</h1>
        <button onClick={handleRecalculate} className="btn-secondary">
          Recalcular
        </button>
      </div>

      <div className="stats-grid">
        <div className="stat-card primary">
          <div className="stat-icon">⚽</div>
          <div className="stat-value">{stats.totalPeladas}</div>
          <div className="stat-label">Peladas Jogadas</div>
        </div>

        <div className="stat-card success">
          <div className="stat-icon">🥅</div>
          <div className="stat-value">{stats.totalGoals}</div>
          <div className="stat-label">Gols Marcados</div>
        </div>

        <div className="stat-card info">
          <div className="stat-icon">📊</div>
          <div className="stat-value">{stats.goalsPerGame.toFixed(2)}</div>
          <div className="stat-label">Média de Gols</div>
        </div>

        <div className="stat-card warning">
          <div className="stat-icon">🏆</div>
          <div className="stat-value">{stats.totalWins}</div>
          <div className="stat-label">Vitórias</div>
        </div>

        <div className="stat-card danger">
          <div className="stat-icon">😔</div>
          <div className="stat-value">{stats.totalLosses}</div>
          <div className="stat-label">Derrotas</div>
        </div>

        <div className="stat-card neutral">
          <div className="stat-icon">🤝</div>
          <div className="stat-value">{stats.totalDraws}</div>
          <div className="stat-label">Empates</div>
        </div>

        <div className="stat-card percentage">
          <div className="stat-icon">📈</div>
          <div className="stat-value">{stats.winRate.toFixed(1)}%</div>
          <div className="stat-label">Taxa de Vitória</div>
        </div>

        <div className="stat-card percentage">
          <div className="stat-icon">✅</div>
          <div className="stat-value">{stats.attendanceRate.toFixed(1)}%</div>
          <div className="stat-label">Taxa de Comparecimento</div>
        </div>

        <div className="stat-card warning">
          <div className="stat-icon">❌</div>
          <div className="stat-value">{stats.totalNoShows}</div>
          <div className="stat-label">Faltas</div>
        </div>
      </div>

      <div className="stats-streaks">
        <div className="streak-card">
          <h3>Sequências</h3>
          <div className="streak-info">
            <div className="streak-item">
              <span className="streak-label">Sequência Atual:</span>
              <span className="streak-value current">{stats.currentWinStreak} vitórias</span>
            </div>
            <div className="streak-item">
              <span className="streak-label">Melhor Sequência:</span>
              <span className="streak-value best">{stats.bestWinStreak} vitórias</span>
            </div>
          </div>
        </div>

        {stats.lastPeladaDate && (
          <div className="last-pelada-card">
            <h3>Última Pelada</h3>
            <p className="last-pelada-date">
              {new Date(stats.lastPeladaDate).toLocaleDateString('pt-BR', {
                day: '2-digit',
                month: 'long',
                year: 'numeric'
              })}
            </p>
          </div>
        )}
      </div>

      <style jsx>{`
        .user-stats-page {
          padding: 2rem;
          max-width: 1200px;
          margin: 0 auto;
        }

        .stats-header {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 2rem;
        }

        .stats-grid {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
          gap: 1.5rem;
          margin-bottom: 2rem;
        }

        .stat-card {
          background: white;
          border-radius: 12px;
          padding: 1.5rem;
          box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
          text-align: center;
          transition: transform 0.2s;
        }

        .stat-card:hover {
          transform: translateY(-4px);
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        }

        .stat-icon {
          font-size: 2.5rem;
          margin-bottom: 0.5rem;
        }

        .stat-value {
          font-size: 2rem;
          font-weight: bold;
          margin-bottom: 0.5rem;
        }

        .stat-label {
          color: #666;
          font-size: 0.9rem;
        }

        .stat-card.primary { border-left: 4px solid #3b82f6; }
        .stat-card.success { border-left: 4px solid #10b981; }
        .stat-card.info { border-left: 4px solid #06b6d4; }
        .stat-card.warning { border-left: 4px solid #f59e0b; }
        .stat-card.danger { border-left: 4px solid #ef4444; }
        .stat-card.neutral { border-left: 4px solid #6b7280; }
        .stat-card.percentage { border-left: 4px solid #8b5cf6; }

        .stats-streaks {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
          gap: 1.5rem;
        }

        .streak-card,
        .last-pelada-card {
          background: white;
          border-radius: 12px;
          padding: 1.5rem;
          box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        }

        .streak-card h3,
        .last-pelada-card h3 {
          margin-bottom: 1rem;
          color: #1f2937;
        }

        .streak-info {
          display: flex;
          flex-direction: column;
          gap: 1rem;
        }

        .streak-item {
          display: flex;
          justify-content: space-between;
          align-items: center;
        }

        .streak-label {
          color: #6b7280;
        }

        .streak-value {
          font-weight: bold;
          font-size: 1.1rem;
        }

        .streak-value.current {
          color: #3b82f6;
        }

        .streak-value.best {
          color: #f59e0b;
        }

        .last-pelada-date {
          font-size: 1.2rem;
          color: #3b82f6;
          font-weight: 500;
        }

        .loading,
        .no-stats {
          text-align: center;
          padding: 3rem;
          font-size: 1.2rem;
          color: #6b7280;
        }
      `}</style>
    </div>
  );
}
