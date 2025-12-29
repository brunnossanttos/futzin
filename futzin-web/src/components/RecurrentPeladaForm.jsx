import { useState } from 'react';

const RecurrenceType = {
  None: 0,
  Daily: 1,
  Weekly: 2,
  BiWeekly: 3,
  Monthly: 4
};

const RecurrenceTypeLabels = {
  0: 'Pelada Única',
  1: 'Diária',
  2: 'Semanal',
  3: 'Quinzenal',
  4: 'Mensal'
};

const DaysOfWeek = [
  { value: 0, label: 'Domingo' },
  { value: 1, label: 'Segunda-feira' },
  { value: 2, label: 'Terça-feira' },
  { value: 3, label: 'Quarta-feira' },
  { value: 4, label: 'Quinta-feira' },
  { value: 5, label: 'Sexta-feira' },
  { value: 6, label: 'Sábado' }
];

export default function RecurrentPeladaForm({ onSubmit, initialData = {} }) {
  const [formData, setFormData] = useState({
    name: initialData.name || '',
    description: initialData.description || '',
    date: initialData.date || '',
    location: initialData.location || '',
    fieldType: initialData.fieldType || 0,
    price: initialData.price || 0,
    maxPlayers: initialData.maxPlayers || 10,
    recurrenceType: initialData.recurrenceType || RecurrenceType.None,
    recurrenceEndDate: initialData.recurrenceEndDate || '',
    dayOfWeek: initialData.dayOfWeek || null,
    dayOfMonth: initialData.dayOfMonth || null,
    groupId: initialData.groupId || null,
    isPublic: initialData.isPublic || false
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value
    });
  };

  const handleRecurrenceTypeChange = (e) => {
    const recurrenceType = parseInt(e.target.value);
    setFormData({
      ...formData,
      recurrenceType,
      dayOfWeek: recurrenceType === RecurrenceType.Weekly || recurrenceType === RecurrenceType.BiWeekly
        ? (formData.dayOfWeek || 0)
        : null,
      dayOfMonth: recurrenceType === RecurrenceType.Monthly
        ? (formData.dayOfMonth || 1)
        : null
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <form onSubmit={handleSubmit} className="recurrent-pelada-form">
      <div className="form-section">
        <h3>Informações Básicas</h3>

        <div className="form-group">
          <label htmlFor="name">Nome da Pelada *</label>
          <input
            id="name"
            name="name"
            type="text"
            value={formData.name}
            onChange={handleChange}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="description">Descrição</label>
          <textarea
            id="description"
            name="description"
            value={formData.description}
            onChange={handleChange}
            rows="3"
          />
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="date">Data Inicial *</label>
            <input
              id="date"
              name="date"
              type="datetime-local"
              value={formData.date}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="location">Local *</label>
            <input
              id="location"
              name="location"
              type="text"
              value={formData.location}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="price">Preço (R$)</label>
            <input
              id="price"
              name="price"
              type="number"
              step="0.01"
              value={formData.price}
              onChange={handleChange}
            />
          </div>

          <div className="form-group">
            <label htmlFor="maxPlayers">Máximo de Jogadores</label>
            <input
              id="maxPlayers"
              name="maxPlayers"
              type="number"
              value={formData.maxPlayers}
              onChange={handleChange}
              min="2"
            />
          </div>
        </div>
      </div>

      <div className="form-section">
        <h3>Recorrência</h3>

        <div className="form-group">
          <label htmlFor="recurrenceType">Tipo de Recorrência</label>
          <select
            id="recurrenceType"
            name="recurrenceType"
            value={formData.recurrenceType}
            onChange={handleRecurrenceTypeChange}
          >
            {Object.entries(RecurrenceTypeLabels).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
        </div>

        {formData.recurrenceType !== RecurrenceType.None && (
          <>
            {(formData.recurrenceType === RecurrenceType.Weekly ||
              formData.recurrenceType === RecurrenceType.BiWeekly) && (
              <div className="form-group">
                <label htmlFor="dayOfWeek">Dia da Semana</label>
                <select
                  id="dayOfWeek"
                  name="dayOfWeek"
                  value={formData.dayOfWeek || ''}
                  onChange={handleChange}
                  required
                >
                  <option value="">Selecione...</option>
                  {DaysOfWeek.map(day => (
                    <option key={day.value} value={day.value}>
                      {day.label}
                    </option>
                  ))}
                </select>
              </div>
            )}

            {formData.recurrenceType === RecurrenceType.Monthly && (
              <div className="form-group">
                <label htmlFor="dayOfMonth">Dia do Mês</label>
                <input
                  id="dayOfMonth"
                  name="dayOfMonth"
                  type="number"
                  min="1"
                  max="31"
                  value={formData.dayOfMonth || ''}
                  onChange={handleChange}
                  required
                />
              </div>
            )}

            <div className="form-group">
              <label htmlFor="recurrenceEndDate">Data de Término da Recorrência</label>
              <input
                id="recurrenceEndDate"
                name="recurrenceEndDate"
                type="date"
                value={formData.recurrenceEndDate}
                onChange={handleChange}
              />
              <small>Deixe em branco para recorrência sem data final</small>
            </div>
          </>
        )}
      </div>

      <div className="form-section">
        <h3>Configurações</h3>

        <div className="form-group checkbox">
          <label>
            <input
              type="checkbox"
              name="isPublic"
              checked={formData.isPublic}
              onChange={handleChange}
            />
            <span>Pelada pública (qualquer um pode ver e entrar)</span>
          </label>
        </div>
      </div>

      <div className="form-actions">
        <button type="submit" className="btn-primary">
          Criar Pelada
        </button>
      </div>

      <style jsx>{`
        .recurrent-pelada-form {
          max-width: 800px;
          margin: 0 auto;
          padding: 2rem;
        }

        .form-section {
          background: white;
          border-radius: 8px;
          padding: 1.5rem;
          margin-bottom: 1.5rem;
          box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        .form-section h3 {
          margin-top: 0;
          margin-bottom: 1.5rem;
          color: #1f2937;
          font-size: 1.25rem;
        }

        .form-group {
          margin-bottom: 1.5rem;
        }

        .form-group label {
          display: block;
          margin-bottom: 0.5rem;
          color: #374151;
          font-weight: 500;
        }

        .form-group input,
        .form-group select,
        .form-group textarea {
          width: 100%;
          padding: 0.75rem;
          border: 1px solid #d1d5db;
          border-radius: 6px;
          font-size: 1rem;
          transition: border-color 0.2s;
        }

        .form-group input:focus,
        .form-group select:focus,
        .form-group textarea:focus {
          outline: none;
          border-color: #3b82f6;
        }

        .form-group small {
          display: block;
          margin-top: 0.25rem;
          color: #6b7280;
          font-size: 0.875rem;
        }

        .form-group.checkbox label {
          display: flex;
          align-items: center;
          cursor: pointer;
        }

        .form-group.checkbox input {
          width: auto;
          margin-right: 0.5rem;
        }

        .form-row {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
          gap: 1rem;
        }

        .form-actions {
          display: flex;
          justify-content: flex-end;
          gap: 1rem;
          margin-top: 2rem;
        }

        .btn-primary {
          background-color: #3b82f6;
          color: white;
          padding: 0.75rem 2rem;
          border: none;
          border-radius: 6px;
          font-size: 1rem;
          font-weight: 500;
          cursor: pointer;
          transition: background-color 0.2s;
        }

        .btn-primary:hover {
          background-color: #2563eb;
        }
      `}</style>
    </form>
  );
}
