import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import RecurrentPeladaForm from '../components/RecurrentPeladaForm';
import { peladaService } from '../services/peladaService';
import { attendanceService } from '../services/attendanceService';

export default function CreatePeladaPage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = async (formData) => {
    setLoading(true);
    setError(null);

    try {
      const pelada = await peladaService.create({
        ...formData,
        date: new Date(formData.date).toISOString()
      });

      if (formData.groupId) {
        // TODO: Buscar membros do grupo e criar confirmações
        // Isso pode ser feito pelo backend automaticamente
      }

      // Redirecionar para a página da pelada
      navigate(`/peladas/${pelada.id}`);
    } catch (err) {
      console.error('Erro ao criar pelada:', err);
      setError('Erro ao criar pelada. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="create-pelada-page">
      <div className="page-header">
        <h1>Criar Nova Pelada</h1>
        <p>Preencha os dados abaixo para criar uma nova pelada</p>
      </div>

      {error && (
        <div className="error-message">
          {error}
        </div>
      )}

      <RecurrentPeladaForm onSubmit={handleSubmit} />

      {loading && (
        <div className="loading-overlay">
          <div className="loading-spinner">Criando pelada...</div>
        </div>
      )}

      <style jsx>{`
        .create-pelada-page {
          min-height: 100vh;
          background: #f3f4f6;
          padding: 2rem 0;
        }

        .page-header {
          text-align: center;
          margin-bottom: 2rem;
        }

        .page-header h1 {
          color: #1f2937;
          margin-bottom: 0.5rem;
        }

        .page-header p {
          color: #6b7280;
        }

        .error-message {
          background-color: #fee2e2;
          color: #991b1b;
          padding: 1rem;
          border-radius: 8px;
          margin: 0 auto 2rem;
          max-width: 800px;
          text-align: center;
        }

        .loading-overlay {
          position: fixed;
          top: 0;
          left: 0;
          right: 0;
          bottom: 0;
          background: rgba(0, 0, 0, 0.5);
          display: flex;
          align-items: center;
          justify-content: center;
          z-index: 1000;
        }

        .loading-spinner {
          background: white;
          padding: 2rem 3rem;
          border-radius: 12px;
          font-size: 1.25rem;
          font-weight: 500;
        }
      `}</style>
    </div>
  );
}
