import React, { useState, useEffect } from 'react';
import httpClient from '../../../data/http/httpClient';
import Pagination from '../../ui/Pagination';
import Button from '../../ui/Button';
import TextField from '../../ui/TextField';
import DepartmentFormDialog from './DepartmentFormDialog';

export default function DepartmentsScreen() {
    const [departments, setDepartments] = useState([]);
    const [page, setPage] = useState(1);
    const [size] = useState(10);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [editingDepartment, setEditingDepartment] = useState(null);

    const loadDepartments = async () => {
        try {
            const response = await httpClient.get(`/api/departments?page=${page}&size=${size}&search=${search}`);
            setDepartments(response.data.items);
            setTotalPages(response.data.totalPages);
        } catch (error) {
            console.error("Error loading departments", error);
        }
    };

    useEffect(() => {
        loadDepartments();
    }, [page, search]);

    const handleDelete = async (id) => {
        if (confirm('Deactivate department?')) {
            await httpClient.delete(`/api/departments/${id}`);
            loadDepartments();
        }
    };

    return (
        <div>
            <h1>Departments</h1>
            <TextField 
                placeholder="Search departments..." 
                value={search} 
                onChange={(e) => { setSearch(e.target.value); setPage(1); }} 
            />
            <Button onClick={() => { setEditingDepartment(null); setIsDialogOpen(true); }}>
                New Department
            </Button>

            <table>
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {departments.map(dep => (
                        <tr key={dep.id}>
                            <td>{dep.name}</td>
                            <td>{dep.active ? 'Active' : 'Inactive'}</td>
                            <td>
                                <Button onClick={() => { setEditingDepartment(dep); setIsDialogOpen(true); }}>Edit</Button>
                                <Button onClick={() => handleDelete(dep.id)}>Deactivate</Button>
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