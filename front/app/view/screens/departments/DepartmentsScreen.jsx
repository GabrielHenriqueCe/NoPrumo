import { useState, useEffect, useCallback } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Pagination } from '../../ui/Pagination';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { DepartmentFormDialog } from './DepartmentFormDialog';
import { useT } from '../../preferences/preferencesContext'

export function DepartmentsScreen() {
    const t = useT()
    const { departments: departmentGateway } = useContainer();
    const [total, setTotal] = useState(0);
    const [departments, setDepartments] = useState([]);
    const [page, setPage] = useState(1);
    const [size] = useState(10);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [editingDepartment, setEditingDepartment] = useState(null);
    const [error, setError] = useState(null);

    const loadDepartments = useCallback(async (signal) => {
        setError(null);
        try {
            const response = await departmentGateway.list({ page, size, search }, {signal});
            setDepartments(response.items);
            setTotalPages(response.totalPages);
            setTotal(response.total);
        } catch (error) {
            if (error.name === 'AbortError' || error.name === 'CanceledError') return;
            console.error("Error loading departments", error);
            setError("Error loading departments. Please try again.");
        }
    }, [page, size, search, departmentGateway]);

    useEffect(() => {
        const controller = new AbortController();

        const delay = setTimeout(() => {
            loadDepartments(controller.signal);
        }, 350);

        return () => {
            clearTimeout(delay);
            controller.abort();
        };
    }, [loadDepartments]);

    const handleToggleActive = async (item) => {
        if (item.active && !window.confirm('Are you sure you want to deactivate this record?')) return;
        setError(null);

        try {
            await departmentGateway.setActive(item.id, !item.active);
            loadDepartments();
        } catch (error) {
            console.error(error);
            setError("Error updating department status. Please try again.");
        }
    };

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-4">{t('departmentsScreen.departments')}</h1>
            {error && (
                <div className="bg-danger-wash border border-late text-danger px-4 py-3 rounded mb-4">
                    {error}
                </div>
            )}
            <div className="flex gap-4 mb-4">
                <TextField 
                    placeholder={t('departmentsScreen.searchDepartments')} 
                    value={search} 
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }} 
                />
                <Button onClick={() => { setEditingDepartment(null); setIsDialogOpen(true); }}>
                    {t('departmentsScreen.newDepartment')}
                </Button>
            </div>

            <table className="w-full text-left border-collapse mb-4">
                <thead>
                    <tr className="border-b">
                        <th className="py-2">{t('common.name')}</th>
                        <th className="py-2">{t('common.status')}</th>
                        <th className="py-2">{t('common.actions')}</th>
                    </tr>
                </thead>
                <tbody>
                    {departments.map(dep => (
                        <tr key={dep.id} className="border-b">
                            <td className="py-2">{dep.name}</td>
                            <td className="py-2">{dep.active ? t('common.active') : t('common.inactive')}</td>
                            <td className="py-2 flex gap-2">
                                <Button variant="outline" onClick={() => { setEditingDepartment(dep); setIsDialogOpen(true); }}>{t('common.edit')}</Button>
                                <Button variant="outline" onClick={() => handleToggleActive(dep)}>
                                    {dep.active ? t('common.deactivate') : t('common.activate')}
                                </Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <Pagination page={page} totalPages={totalPages} total={total} onChange={setPage} />

            {isDialogOpen && (
                <DepartmentFormDialog 
                    department={editingDepartment} 
                    onClose={() => setIsDialogOpen(false)} 
                    onSave={() => { setIsDialogOpen(false); loadDepartments(); }} 
                />
            )}
        </div>
    );
}