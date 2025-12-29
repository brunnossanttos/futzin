import { useState, useEffect } from 'react';
import { attendanceService } from '../services/attendanceService';

const AttendanceStatus = {
  0: 'Pendente',
  1: 'Confirmado',
  2: 'Recusado',
  3: 'Compareceu',
  4: 'Faltou'
};

const AttendanceStatusColor = {
  0: 'gray',
  1: 'green',
  2: 'red',
  3: 'blue',
  4: 'orange'
};

export default function AttendanceManager({ peladaId, userId, isOrganizer }) {
  const [attendances, setAttendances] = useState([]);
  const [userAttendance, setUserAttendance] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadAttendances();
  }, [peladaId]);

  const loadAttendances = async () => {
    try {
      setLoading(true);
      const data = await attendanceService.getPeladaAttendances(peladaId);
      setAttendances(data);

      const myAttendance = data.find(a => a.userId === userId);
      setUserAttendance(myAttendance);
    } catch (error) {
      console.error('Erro ao carregar confirmações:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleConfirm = async () => {
    try {
      await attendanceService.confirm(peladaId, userId);
      loadAttendances();
    } catch (error) {
      console.error('Erro ao confirmar presença:', error);
    }
  };

  const handleDecline = async () => {
    try {
      await attendanceService.decline(peladaId, userId);
      loadAttendances();
    } catch (error) {
      console.error('Erro ao recusar presença:', error);
    }
  };

  const handleMarkAttended = async (attendanceUserId) => {
    try {
      await attendanceService.markAttended(peladaId, attendanceUserId);
      loadAttendances();
    } catch (error) {
      console.error('Erro ao marcar como compareceu:', error);
    }
  };

  const handleMarkNoShow = async (attendanceUserId) => {
    try {
      await attendanceService.markNoShow(peladaId, attendanceUserId);
      loadAttendances();
    } catch (error) {
      console.error('Erro ao marcar como faltou:', error);
    }
  };

  const getStatusCount = (status) => {
    return attendances.filter(a => a.status === status).length;
  };

  if (loading) {
    return <div>Carregando...</div>;
  }

  return (
    <div className="attendance-manager">
      <div className="attendance-summary">
        <h3>Confirmações de Presença</h3>
        <div className="status-counts">
          <div className="status-badge confirmed">
            Confirmados: {getStatusCount(1)}
          </div>
          <div className="status-badge declined">
            Recusados: {getStatusCount(2)}
          </div>
          <div className="status-badge pending">
            Pendentes: {getStatusCount(0)}
          </div>
        </div>
      </div>

      {userAttendance && (
        <div className="my-attendance">
          <h4>Sua Confirmação</h4>
          <p>
            Status:
            <span
              className="status-badge"
              style={{ backgroundColor: AttendanceStatusColor[userAttendance.status] }}
            >
              {AttendanceStatus[userAttendance.status]}
            </span>
          </p>
          {userAttendance.status === 0 && (
            <div className="attendance-actions">
              <button onClick={handleConfirm} className="btn-success">
                Confirmar Presença
              </button>
              <button onClick={handleDecline} className="btn-danger">
                Não Vou
              </button>
            </div>
          )}
        </div>
      )}

      <div className="attendances-list">
        <h4>Participantes</h4>
        <ul>
          {attendances.map(attendance => (
            <li key={attendance.id} className="attendance-item">
              <div className="user-info">
                <span className="user-name">{attendance.user?.name}</span>
                <span
                  className="status-badge"
                  style={{ backgroundColor: AttendanceStatusColor[attendance.status] }}
                >
                  {AttendanceStatus[attendance.status]}
                </span>
              </div>
              {isOrganizer && attendance.status === 1 && (
                <div className="organizer-actions">
                  <button
                    onClick={() => handleMarkAttended(attendance.userId)}
                    className="btn-sm btn-success"
                  >
                    Compareceu
                  </button>
                  <button
                    onClick={() => handleMarkNoShow(attendance.userId)}
                    className="btn-sm btn-warning"
                  >
                    Faltou
                  </button>
                </div>
              )}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
