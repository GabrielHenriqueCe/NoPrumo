import { useState, useEffect, useCallback } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Pagination } from '../../ui/Pagination';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { EmployeeFormDialog } from './EmployeeFormDialog';
import { EmployeeProfile } from './EmployeeProfile';
import { useSession } from '../../providers/sessionContext';
import { useT } from '../../preferences/preferencesContext'

export function EmployeesScreen() {
    const t = useT()
    const { can } = useSession();
    const canViewFinance = can('view_finance');
    const { employees } = useContainer();

    const [employeesList, setEmployeesList] = useState([]);
    const [page, setPage] = useState(1);
    const [size] = useState(10);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [total, setTotal] = useState(0);
    const [error, setError] = useState(null);

    // Estados para as Dialogs
    const [isFormDialogOpen, setIsFormDialogOpen] = useState(false);
    const [editingEmployee, setEditingEmployee] = useState(null);
    const [viewingEmployeeId, setViewingEmployeeId] = useState(null);

    const loadEmployees = useCallback(async (signal) => {
        setError(null);
        try {
            const response = await employees.list({ page, size, search }, { signal });
            setEmployeesList(response.items);
            setTotalPages(response.totalPages);
            setTotal(response.total);
        } catch (error) {
            if (error.name === 'AbortError' || error.name === 'CanceledError') return;
            console.error("Error loading employees", error);
            setError("Error loading employees. Please try again.");
        }
    }, [employees, page, size, search]);

    useEffect(() => {
        const controller = new AbortController();

        const delay = setTimeout(() => {
            loadEmployees(controller.signal);
        }, 350);

        return () => {
            clearTimeout(delay);
            controller.abort();
        };
    }, [loadEmployees]);

    const handleToggleActive = async (item) => {
        if (item.active && !window.confirm('Are you sure you want to deactivate this record?')) return;
        setError(null);
        try {
            await employees.setActive(item.id, !item.active);
            loadEmployees();
        } catch (error) {
            console.error(error);
            setError("Error updating employee status. Please try again.");
        }
    };

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-4">{t('employeesScreen.employees')}</h1>
            {error && (
                <div className="bg-danger-wash border border-late text-danger px-4 py-3 rounded mb-4">
                    {error}
                </div>
            )}

            <div className="flex gap-4 mb-4">
                <TextField
                    placeholder={t('employeesScreen.searchByNameOrRegistration')}
                    value={search}
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }}
                />
                <Button onClick={() => { setEditingEmployee(null); setIsFormDialogOpen(true); }}>
                    {t('employeesScreen.newEmployee')}
                </Button>
            </div>

            <table className="w-full text-left border-collapse mb-4">
                <thead>
                    <tr className="border-b bg-sunken">
                        <th className="py-3 px-2">{t('employeesScreen.regNumber')}</th>
                        <th className="py-3 px-2">{t('common.name')}</th>
                        <th className="py-3 px-2">{t('employeesScreen.jobRole')}</th>
                        <th className="py-3 px-2">{t('employeesScreen.regime')}</th>
                        <th className="py-3 px-2">{t('common.status')}</th>
                        <th className="py-3 px-2 text-right">{t('common.actions')}</th>
                    </tr>
                </thead>
                <tbody>
                    {employeesList.map(emp => (
                        <tr key={emp.id} className="border-b hover:bg-sunken transition-colors">
                            <td className="py-3 px-2 text-sm">{emp.registrationNumber || '-'}</td>

                            {/* CLICAR NO NOME ABRE A FICHA */}
                            <td className="py-3 px-2">
                                <button
                                    className="font-medium text-accent hover:text-accent hover:underline text-left"
                                    onClick={() => setViewingEmployeeId(emp.id)}
                                >
                                    {emp.name}
                                </button>
                            </td>

                            <td className="py-3 px-2 text-sm">{emp.jobRoleName || '-'}</td>
                            <td className="py-3 px-2 text-sm">{emp.employmentRegimeLabel || '-'}</td>
                            <td className="py-3 px-2">
                                <span className={`px-2 py-1 text-xs rounded-full ${emp.active ? 'bg-success-wash text-success' : 'bg-sunken text-ink'}`}>
                                    {emp.active ? t('common.active') : t('common.inactive')}
                                </span>
                            </td>
                            <td className="py-3 px-2 flex gap-2 justify-end">
                                <Button variant="outline" size="sm" onClick={() => { setEditingEmployee(emp); setIsFormDialogOpen(true); }}>{t('common.edit')}</Button>
                                <Button variant="outline" size="sm" onClick={() => handleToggleActive(emp)}>
                                    {emp.active ? t('common.deactivate') : t('common.activate')}
                                </Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {employeesList.length === 0 && !error && (
                <div className="text-center py-8 text-muted">{t('employeesScreen.noEmployeesFound')}</div>
            )}

            <Pagination page={page} totalPages={totalPages} total={total} onChange={setPage} />

            {/* Modal de Criação/Edição */}
            {isFormDialogOpen && (
                <EmployeeFormDialog
                    employee={editingEmployee}
                    onClose={() => setIsFormDialogOpen(false)}
                    onSave={() => { setIsFormDialogOpen(false); loadEmployees(); }}
                    canViewFinance={canViewFinance}
                />
            )}

            {/* Modal da Ficha do Funcionário (Profile) */}
            {viewingEmployeeId && (
                <EmployeeProfile
                    employeeId={viewingEmployeeId}
                    onClose={() => setViewingEmployeeId(null)}
                />
            )}
        </div>
    );
}