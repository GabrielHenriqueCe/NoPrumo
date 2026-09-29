import React, { useState, useEffect } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Dialog } from '../../ui/Dialog';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';

export function JobRoleFormDialog({ jobRole, onClose, onSave }) {
    const { jobRoles, departments } = useContainer();
    const [name, setName] = useState(jobRole ? jobRole.name : '');
    const [departmentId, setDepartmentId] = useState(jobRole ? jobRole.departmentId : '');
    const [active, setActive] = useState(jobRole ? jobRole.active : true);
    
    const [departmentsList, setDepartmentsList] = useState([]);
    const [errors, setErrors] = useState(null);

    // Carrega a lista de Setores para o Select
    useEffect(() => {
        const fetchDepartments = async () => {
            try {
                const response = await departments.list({ size: 1000 });
                // Filtramos apenas os ativos para o dropdown de criação,
                // mas garantimos que o departamento atual apareça na edição mesmo se inativo.
                const activeDeps = response.items.filter(d => d.active || (jobRole && d.id === jobRole.departmentId));
                setDepartmentsList(activeDeps);
                
                if (!jobRole && activeDeps.length > 0) {
                    setDepartmentId(activeDeps[0].id);
                }
            } catch (error) {
                console.error("Error loading departments for select", error);
            }
        };
        fetchDepartments();
    }, [departments, jobRole]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErrors(null);
        try {
            const payload = { 
                name, 
                departmentId: Number(departmentId), 
                active 
            };

            if (jobRole) {
                await jobRoles.update(jobRole.id, payload);
            } else {
                await jobRoles.create(payload);
            }
            onSave();
        } catch (error) {
            if (error.payload && error.payload.errors) {
                setErrors(error.payload.errors);
            } else if (error.errors) {
                setErrors(error.errors);
            }
        }
    };

    return (
        <Dialog open={true} title={jobRole ? "Edit Job Role" : "New Job Role"} onClose={onClose}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                <TextField 
                    label="Name" 
                    value={name} 
                    onChange={e => setName(e.target.value)} 
                    error={errors?.Name?.join(', ')}
                />
                
                <div className="flex flex-col">
                    <label className="text-sm font-semibold mb-1">Department</label>
                    <select 
                        value={departmentId} 
                        onChange={e => setDepartmentId(e.target.value)}
                        className="border border-line rounded px-3 py-2 outline-none focus:border-graphite"
                    >
                        {departmentsList.map(dep => (
                            <option key={dep.id} value={dep.id}>{dep.name}</option>
                        ))}
                    </select>
                    {errors?.DepartmentId && <span className="text-danger text-sm mt-1">{errors.DepartmentId.join(', ')}</span>}
                </div>
                
                {jobRole && (
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