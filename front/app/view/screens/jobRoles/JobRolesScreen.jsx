import { useState, useEffect, useCallback } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Pagination } from '../../ui/Pagination';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { JobRoleFormDialog } from './JobRoleFormDialog';
import { useT } from '../../preferences/preferencesContext'

export function JobRolesScreen() {
    const t = useT()
    const { jobRoles, departments } = useContainer();
    const [rolesList, setRolesList] = useState([]);
    const [departmentsMap, setDepartmentsMap] = useState({});
    const [total, setTotal] = useState(0);
    const [page, setPage] = useState(1);
    const [size] = useState(10);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [error, setError] = useState(null);
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [editingRole, setEditingRole] = useState(null);

    // Carrega a lista de Setores para exibir o Nome em vez do ID na tabela
    useEffect(() => {
        const fetchDepartments = async () => {
            try {
                // Busca todos os departamentos para o mapeamento
                const response = await departments.list({ size: 1000 });
                const map = {};
                response.items.forEach(d => {
                    map[d.id] = d.name;
                });
                setDepartmentsMap(map);
            } catch (error) {
                console.error("Error loading departments for map", error);
            }
        };
        fetchDepartments();
    }, [departments]);

    const loadRoles = useCallback(async (signal) => {
        setError(null);
        try {
            const response = await jobRoles.list({ page, size, search }, {signal});
            setRolesList(response.items);
            setTotalPages(response.totalPages);
            setTotal(response.total);
        } catch (error) {
            if (error.name === 'AbortError' || error.name === 'CanceledError') return;
            console.error("Error loading job roles", error);
            setError("Error loading job roles. Please try again.");
        }
    }, [page, size, search, jobRoles]
);

    useEffect(() => {
        const controller = new AbortController();

        const delay = setTimeout(() => {
            loadRoles(controller.signal);
        }, 350);

        return () => {
            clearTimeout(delay);
            controller.abort();
        };
    }, [loadRoles]);

    const handleToggleActive = async (item) => {
        if (item.active && !window.confirm('Are you sure you want to deactivate this record?')) return;
        setError(null);

        try {
            await jobRoles.setActive(item.id, !item.active);

            loadRoles();
        } catch (error) {
            console.error(error);
            setError("Error updating job role status. Please try again.");
        }
    };

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-4">{t('jobRolesScreen.jobRoles')}</h1>
            {error && (
                <div className="bg-danger-wash border border-late text-danger px-4 py-3 rounded mb-4">
                    {error}
                </div>
            )}
            <div className="flex gap-4 mb-4">
                <TextField 
                    placeholder={t('jobRolesScreen.searchJobRoles')} 
                    value={search} 
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }} 
                />
                <Button onClick={() => { setEditingRole(null); setIsDialogOpen(true); }}>
                    {t('jobRolesScreen.newJobRole')}
                </Button>
            </div>

            <table className="w-full text-left border-collapse mb-4">
                <thead>
                    <tr className="border-b">
                        <th className="py-2">{t('common.name')}</th>
                        <th className="py-2">{t('jobRolesScreen.department')}</th>
                        <th className="py-2">{t('common.status')}</th>
                        <th className="py-2">{t('common.actions')}</th>
                    </tr>
                </thead>
                <tbody>
                    {rolesList.map(role => (
                        <tr key={role.id} className="border-b">
                            <td className="py-2">{role.name}</td>
                            <td className="py-2">{departmentsMap[role.departmentId] || role.departmentId}</td>
                            <td className="py-2">{role.active ? t('common.active') : t('common.inactive')}</td>
                            <td className="py-2 flex gap-2">
                                <Button variant="outline" onClick={() => { setEditingRole(role); setIsDialogOpen(true); }}>{t('common.edit')}</Button>
                                <Button variant="outline" onClick={() => handleToggleActive(role)}>
                                    {role.active ? t('common.deactivate') : t('common.activate')}
                                </Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <Pagination page={page} totalPages={totalPages} total={total} onChange={setPage} />

            {isDialogOpen && (
                <JobRoleFormDialog 
                    jobRole={editingRole} 
                    onClose={() => setIsDialogOpen(false)} 
                    onSave={() => { setIsDialogOpen(false); loadRoles(); }} 
                />
            )}
        </div>
    );
}