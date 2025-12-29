import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { peladasAPI } from '../../services/api';
import Input from '../../components/common/Input';
import Button from '../../components/common/Button';
import Card from '../../components/common/Card';
import './PeladaForm.css';

const PeladaForm = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = !!id;

  const [formData, setFormData] = useState({
    name: '',
    description: '',
    date: '',
    location: '',
    fieldType: 0,
    price: '',
    maxPlayers: '',
    isPublic: false,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (isEdit) {
      fetchPelada();
    }
  }, [id]);

  const fetchPelada = async () => {
    try {
      const response = await peladasAPI.getById(id);
      const pelada = response.data;

      const date = new Date(pelada.date);
      const formattedDate = date.toISOString().slice(0, 16);

      setFormData({
        name: pelada.name,
        description: pelada.description || '',
        date: formattedDate,
        location: pelada.location,
        fieldType: pelada.fieldType,
        price: pelada.price.toString(),
        maxPlayers: pelada.maxPlayers.toString(),
        isPublic: pelada.isPublic,
      });
    } catch (error) {
      console.error('Erro ao carregar pelada:', error);
      setError('Erro ao carregar dados da pelada');
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const data = {
        ...formData,
        date: new Date(formData.date).toISOString(),
        price: parseFloat(formData.price),
        maxPlayers: parseInt(formData.maxPlayers),
        fieldType: parseInt(formData.fieldType),
      };

      if (isEdit) {
        await peladasAPI.update(id, data);
      } else {
        await peladasAPI.create(data);
      }

      navigate('/dashboard');
    } catch (error) {
      console.error('Erro ao salvar pelada:', error);
      setError(error.response?.data?.message || 'Erro ao salvar pelada');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="pelada-form-container">
      <div className="pelada-form-box">
        <Button size="small" onClick={() => navigate('/dashboard')}>
          ← Voltar
        </Button>

        <Card>
          <h2 className="form-title">{isEdit ? 'Editar Pelada' : 'Nova Pelada'}</h2>

          {error && <div className="error-message">{error}</div>}

          <form onSubmit={handleSubmit}>
            <Input
              label="Nome da Pelada"
              name="name"
              value={formData.name}
              onChange={handleChange}
              placeholder="Ex: Pelada de Sábado"
              required
            />

            <div className="form-group">
              <label className="input-label">Descrição</label>
              <textarea
                className="input textarea"
                name="description"
                value={formData.description}
                onChange={handleChange}
                placeholder="Adicione detalhes sobre a pelada..."
                rows="3"
              />
            </div>

            <Input
              label="Data e Hora"
              type="datetime-local"
              name="date"
              value={formData.date}
              onChange={handleChange}
              required
            />

            <Input
              label="Local"
              name="location"
              value={formData.location}
              onChange={handleChange}
              placeholder="Ex: Campo do Bairro X"
              required
            />

            <div className="form-group">
              <label className="input-label">Tipo de Campo *</label>
              <select
                className="input"
                name="fieldType"
                value={formData.fieldType}
                onChange={handleChange}
                required
              >
                <option value="0">Campo</option>
                <option value="1">Quadra</option>
                <option value="2">Society</option>
                <option value="3">Terrão</option>
              </select>
            </div>

            <div className="form-row">
              <Input
                label="Preço (R$)"
                type="number"
                name="price"
                value={formData.price}
                onChange={handleChange}
                placeholder="20.00"
                step="0.01"
                min="0"
                required
              />

              <Input
                label="Máximo de Jogadores"
                type="number"
                name="maxPlayers"
                value={formData.maxPlayers}
                onChange={handleChange}
                placeholder="10"
                min="2"
                step="2"
                required
              />
            </div>

            <div className="checkbox-group">
              <label className="checkbox-label">
                <input
                  type="checkbox"
                  name="isPublic"
                  checked={formData.isPublic}
                  onChange={handleChange}
                />
                <span>Pelada pública (qualquer pessoa pode entrar)</span>
              </label>
              <p className="checkbox-help">
                Se desmarcado, apenas pessoas com o link de convite ou convidadas diretamente poderão
                participar.
              </p>
            </div>

            <div className="form-actions">
              <Button type="button" variant="danger" onClick={() => navigate('/dashboard')}>
                Cancelar
              </Button>
              <Button type="submit" loading={loading}>
                {isEdit ? 'Salvar Alterações' : 'Criar Pelada'}
              </Button>
            </div>
          </form>
        </Card>
      </div>
    </div>
  );
};

export default PeladaForm;
