import React, { useState, useEffect } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Pagination } from '../../ui/Pagination';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { JobRoleFormDialog } from './JobRoleFormDialog';

export function JobRolesScreen() {
    const { jobRoles, departments } = useContainer();
    const [rolesList, setRolesList] = useState([]);
    const [departmentsMap, setDepartmentsMap] = useState({});
    const [total, setTotal] = useState(0);
    const [page, setPage] = useState(1);
    const [size] = useState(10);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    
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

    const loadRoles = async () => {
        try {
            const response = await jobRoles.list({ page, size, search });
            setRolesList(response.items);
            setTotalPages(response.totalPages);
            setTotal(response.total);
        } catch (error) {
            console.error("Error loading job roles", error);
        }
    };

    useEffect(() => {
        loadRoles();
    }, [page, search, jobRoles]);

    const handleDelete = async (id) => {
        if (confirm('Deactivate job role?')) {
            // O gateway de JobRoles precisa ter o método remove configurado, faremos isso a seguir
            await jobRoles.remove(id);
            loadRoles();
        }
    };

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-4">Job Roles</h1>
            
            <div className="flex gap-4 mb-4">
                <TextField 
                    placeholder="Search job roles..." 
                    value={search} 
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }} 
                />
                <Button onClick={() => { setEditingRole(null); setIsDialogOpen(true); }}>
                    New Job Role
                </Button>
            </div>

            <table className="w-full text-left border-collapse mb-4">
                <thead>
                    <tr className="border-b">
                        <th className="py-2">Name</th>
                        <th className="py-2">Department</th>
                        <th className="py-2">Status</th>
                        <th className="py-2">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {rolesList.map(role => (
                        <tr key={role.id} className="border-b">
                            <td className="py-2">{role.name}</td>
                            <td className="py-2">{departmentsMap[role.departmentId] || role.departmentId}</td>
                            <td className="py-2">{role.active ? 'Active' : 'Inactive'}</td>
                            <td className="py-2 flex gap-2">
                                <Button variant="outline" onClick={() => { setEditingRole(role); setIsDialogOpen(true); }}>Edit</Button>
                                <Button variant="danger" onClick={() => handleDelete(role.id)}>Deactivate</Button>
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