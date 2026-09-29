import React, { useState } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Dialog } from '../../ui/Dialog';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';

export function DepartmentFormDialog({ department, onClose, onSave }) {
    const { departments: departmentGateway } = useContainer();
    const [name, setName] = useState(department ? department.name : '');
    const [active, setActive] = useState(department ? department.active : true);
    const [errors, setErrors] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErrors(null);
        try {
            if (department) {
                await departmentGateway.update(department.id, { name, active });
            } else {
                await departmentGateway.create({ name });
            }
            onSave();
        } catch (error) {
            // O tratamento de erro do httpClient (problemDetails) injeta os erros no payload
            if (error.payload && error.payload.errors) {
                setErrors(error.payload.errors);
            } else if (error.errors) {
                setErrors(error.errors);
            }
        }
    };

    return (
        <Dialog open={true} title={department ? "Edit Department" : "New Department"} onClose={onClose}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                <TextField 
                    label="Name" 
                    value={name} 
                    onChange={e => setName(e.target.value)} 
                    error={errors?.Name?.join(', ')}
                />
                
                {department && (
                    <label className="flex items-center gap-2">
                        <input type="checkbox" checked={active} onChange={e => setActive(e.target.checked)} />
                        Active
                    </label>
                )}
                
                <div className="flex gap-2 mt-4 justify-end">
                    <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
                    <Button type="submit">Save</Button>
                </div>
            </form>
        </Dialog>
    );
}