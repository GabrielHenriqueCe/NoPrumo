import React, { useState } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Dialog } from '../../ui/Dialog';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';

export function DepartmentFormDialog({ department, onClose, onSave }) {
    const { departments: departmentGateway } = useContainer();
    const [name, setName] = useState(department ? department.name : '');
    const [active, setActive] = useState(department ? department.active : true);
    const [fieldErrors, setFieldErrors] = useState({});
    const [formError, setFormError] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setFieldErrors({});
        setFormError(null);
        try {
            if (department) {
                await departmentGateway.update(department.id, { name, active });
            } else {
                await departmentGateway.create({ name });
            }
            onSave();
        } catch (error) {
            if (error.isValidation) {
                setFieldErrors(error.fieldErrors);
            } else {
                setFormError(error.message || 'An unexpected error occurred.');
            }
        }
    };

    return (
        <Dialog open={true} title={department ? "Edit Department" : "New Department"} onClose={onClose}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                {formError && <div className="p-3 bg-red-100 text-red-700 rounded text-sm">{formError}</div>}
                <TextField 
                    label="Name" 
                    value={name} 
                    onChange={e => setName(e.target.value)} 
                    error={fieldErrors.name}
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