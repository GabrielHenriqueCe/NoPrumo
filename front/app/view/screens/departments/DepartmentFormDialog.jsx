import React, { useState } from 'react';
import httpClient from '../../../data/http/httpClient';
import Dialog from '../../ui/Dialog';
import Button from '../../ui/Button';
import TextField from '../../ui/TextField';

export default function DepartmentFormDialog({ department, onClose, onSave }) {
    const [name, setName] = useState(department ? department.name : '');
    const [active, setActive] = useState(department ? department.active : true);
    const [errors, setErrors] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErrors(null);
        try {
            if (department) {
                await httpClient.put(`/api/departments/${department.id}`, { name, active });
            } else {
                await httpClient.post('/api/departments', { name });
            }
            onSave();
        } catch (error) {
            if (error.response && error.response.data && error.response.data.errors) {
                setErrors(error.response.data.errors);
            }
        }
    };

    return (
        <Dialog title={department ? "Edit Department" : "New Department"} onClose={onClose}>
            <form onSubmit={handleSubmit}>
                <TextField 
                    label="Name" 
                    value={name} 
                    onChange={e => setName(e.target.value)} 
                    error={errors?.Name}
                />
                {department && (
                    <label>
                        <input type="checkbox" checked={active} onChange={e => setActive(e.target.checked)} />
                        Active
                    </label>
                )}
                <div>
                    <Button type="submit">Save</Button>
                    <Button type="button" onClick={onClose}>Cancel</Button>
                </div>
            </form>
        </Dialog>
    );
}