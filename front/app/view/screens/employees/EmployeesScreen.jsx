import { useState, useEffect, useCallback} from 'react';
import { useContainer } from '../../providers/containerContext';
import { Pagination } from '../../ui/Pagination';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { EmployeeFormDialog } from './EmployeeFormDialog';
import { useSession } from '../../providers/sessionContext';

export function EmployeesScreen() {
    const { can } = useSession();
    const canViewFinance = can('view_finance');
    const { employees } = useContainer();
    const [employeesList, setEmployeesList] = useState([]);
    const [page, setPage] = useState(1);
    const [size] = useState(10);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [editingEmployee, setEditingEmployee] = useState(null);
    const [total, setTotal] = useState(0);
    const [error, setError] = useState(null);

    const loadEmployees = useCallback(async (signal) => {
        setError(null);
        try {
            const response = await employees.list({ page, size, search }, {signal});
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
            <h1 className="text-2xl font-bold mb-4">Employees</h1>
            {error && (
                <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
                    {error}
                </div>
            )}
            
            <div className="flex gap-4 mb-4">
                <TextField 
                    placeholder="Search by name or registration..." 
                    value={search} 
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }} 
                />
                <Button onClick={() => { setEditingEmployee(null); setIsDialogOpen(true); }}>
                    New Employee
                </Button>
            </div>

            <table className="w-full text-left border-collapse mb-4">
                <thead>
                    <tr className="border-b">
                        <th className="py-2">Reg. Number</th>
                        <th className="py-2">Name</th>
                        <th className="py-2">Job Role</th>
                        <th className="py-2">Regime</th>
                        <th className="py-2">Status</th>
                        <th className="py-2">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {employeesList.map(emp => (
                        <tr key={emp.id} className="border-b">
                            <td className="py-2">{emp.registrationNumber || '-'}</td>
                            <td className="py-2">{emp.name}</td>
                            <td className="py-2">{emp.jobRoleName || '-'}</td>
                            <td className="py-2">{emp.employmentRegimeLabel || '-'}</td>
                            <td className="py-2">{emp.active ? 'Active' : 'Inactive'}</td>
                            <td className="py-2 flex gap-2">
                                <Button variant="outline" onClick={() => { setEditingEmployee(emp); setIsDialogOpen(true); }}>Edit</Button>
                                <Button variant="outline" onClick={() => handleToggleActive(emp)}>
                                    {emp.active ? "Deactivate" : "Activate"}
                                </Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <Pagination page={page} totalPages={totalPages} total={total} onChange={setPage} />

            {isDialogOpen && (
                <EmployeeFormDialog 
                    employee={editingEmployee} 
                    onClose={() => setIsDialogOpen(false)} 
                    onSave={() => { setIsDialogOpen(false); loadEmployees(); }} 
                    canViewFinance={canViewFinance}
                />
            )}
        </div>
    );
}