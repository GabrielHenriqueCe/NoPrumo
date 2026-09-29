import React, { useState, useEffect } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Pagination } from '../../ui/Pagination';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { DepartmentFormDialog } from './DepartmentFormDialog';

export function DepartmentsScreen() {
    // Acedemos ao gateway através do contentor de injeção de dependências
    const { departments: departmentGateway } = useContainer();
    
    const [departments, setDepartments] = useState([]);
    const [page, setPage] = useState(1);
    const [size] = useState(10);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [editingDepartment, setEditingDepartment] = useState(null);

    const loadDepartments = async () => {
        try {
            const response = await departmentGateway.list({ page, size, search });
            // O gateway retorna diretamente o payload (items, page, size, total, totalPages)
            setDepartments(response.items);
            setTotalPages(response.totalPages);
        } catch (error) {
            console.error("Error loading departments", error);
        }
    };

    useEffect(() => {
        loadDepartments();
    }, [page, search, departmentGateway]);

    const handleDelete = async (id) => {
        if (confirm('Deactivate department?')) {
            await departmentGateway.remove(id);
            loadDepartments();
        }
    };

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-4">Departments</h1>
            
            <div className="flex gap-4 mb-4">
                <TextField 
                    placeholder="Search departments..." 
                    value={search} 
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }} 
                />
                <Button onClick={() => { setEditingDepartment(null); setIsDialogOpen(true); }}>
                    New Department
                </Button>
            </div>

            <table className="w-full text-left border-collapse mb-4">
                <thead>
                    <tr className="border-b">
                        <th className="py-2">Name</th>
                        <th className="py-2">Status</th>
                        <th className="py-2">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {departments.map(dep => (
                        <tr key={dep.id} className="border-b">
                            <td className="py-2">{dep.name}</td>
                            <td className="py-2">{dep.active ? 'Active' : 'Inactive'}</td>
                            <td className="py-2 flex gap-2">
                                <Button variant="outline" onClick={() => { setEditingDepartment(dep); setIsDialogOpen(true); }}>Edit</Button>
                                <Button variant="danger" onClick={() => handleDelete(dep.id)}>Deactivate</Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <Pagination current={page} total={totalPages} onPageChange={setPage} />

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