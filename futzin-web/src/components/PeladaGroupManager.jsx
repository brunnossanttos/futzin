import { useState, useEffect } from 'react';
import { peladaGroupService } from '../services/peladaGroupService';

export default function PeladaGroupManager({ userId }) {
  const [groups, setGroups] = useState([]);
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    description: ''
  });

  useEffect(() => {
    loadGroups();
  }, [userId]);

  const loadGroups = async () => {
    try {
      const data = await peladaGroupService.getUserGroups(userId);
      setGroups(data);
    } catch (error) {
      console.error('Erro ao carregar grupos:', error);
    }
  };

  const handleCreateGroup = async (e) => {
    e.preventDefault();
    try {
      await peladaGroupService.create({
        ...formData,
        createdById: userId
      });
      setShowCreateModal(false);
      setFormData({ name: '', description: '' });
      loadGroups();
    } catch (error) {
      console.error('Erro ao criar grupo:', error);
    }
  };

  const handleAddMember = async (groupId, newUserId) => {
    try {
      await peladaGroupService.addMember(groupId, newUserId);
      loadGroups();
    } catch (error) {
      console.error('Erro ao adicionar membro:', error);
    }
  };

  const handleRemoveMember = async (groupId, memberUserId) => {
    try {
      await peladaGroupService.removeMember(groupId, memberUserId);
      loadGroups();
    } catch (error) {
      console.error('Erro ao remover membro:', error);
    }
  };

  return (
    <div className="pelada-group-manager">
      <div className="header">
        <h2>Meus Grupos</h2>
        <button onClick={() => setShowCreateModal(true)} className="btn-primary">
          Criar Novo Grupo
        </button>
      </div>

      <div className="groups-list">
        {groups.map(group => (
          <div key={group.id} className="group-card">
            <h3>{group.name}</h3>
            {group.description && <p>{group.description}</p>}
            <div className="members">
              <h4>Membros ({group.members?.length || 0})</h4>
              <ul>
                {group.members?.map(member => (
                  <li key={member.id}>
                    {member.user.name}
                    {member.isAdmin && <span className="badge">Admin</span>}
                  </li>
                ))}
              </ul>
            </div>
            <button onClick={() => setSelectedGroup(group)}>
              Gerenciar
            </button>
          </div>
        ))}
      </div>

      {showCreateModal && (
        <div className="modal">
          <div className="modal-content">
            <h3>Criar Novo Grupo</h3>
            <form onSubmit={handleCreateGroup}>
              <div className="form-group">
                <label>Nome do Grupo</label>
                <input
                  type="text"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                  required
                />
              </div>
              <div className="form-group">
                <label>Descrição</label>
                <textarea
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                />
              </div>
              <div className="form-actions">
                <button type="submit" className="btn-primary">Criar</button>
                <button type="button" onClick={() => setShowCreateModal(false)} className="btn-secondary">
                  Cancelar
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
