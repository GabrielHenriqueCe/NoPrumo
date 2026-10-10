import React, { useState, useEffect } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Dialog } from '../../ui/Dialog';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';

export function EmployeeFormDialog({ employee, onClose, onSave, canViewFinance }) { 
    const { employees, jobRoles, employmentRegimes } = useContainer();
    
    const [name, setName] = useState(employee ? employee.name : '');
    const [registrationNumber, setRegistrationNumber] = useState(employee && employee.registrationNumber ? employee.registrationNumber : '');
    const [jobRoleId, setJobRoleId] = useState(employee && employee.jobRoleId ? employee.jobRoleId : '');
    const [employmentRegimeId, setEmploymentRegimeId] = useState(employee && employee.employmentRegimeId ? employee.employmentRegimeId : '');
    
    // Campos Financeiros e Adicionais
    const [payRate, setPayRate] = useState(employee && employee.payRate !== undefined ? employee.payRate : '');
    const [additionalPercentage, setAdditionalPercentage] = useState(employee && employee.additionalPercentage !== undefined ? employee.additionalPercentage : '');
    const [hireDate, setHireDate] = useState(employee && employee.hireDate ? employee.hireDate : '');
    const [phone, setPhone] = useState(employee && employee.phone ? employee.phone : '');
    const [document, setDocument] = useState(''); // Documento vem limpo no frontend
    
    const [active, setActive] = useState(employee ? employee.active : true);
    const [fieldErrors, setFieldErrors] = useState({});
    const [formError, setFormError] = useState(null);

    const [rolesList, setRolesList] = useState([]);
    const [regimesList, setRegimesList] = useState([]);

    // Carrega as dependências
    useEffect(() => {
        const fetchDependencies = async () => {
            try {
                const [rolesRes, regimesRes] = await Promise.all([
                    jobRoles.list({ size: 100 }),
                    employmentRegimes.list({ size: 100 })
                ]);
                setRolesList(rolesRes.items);
                setRegimesList(regimesRes.items);
            } catch (error) {
                console.error("Error loading dependencies for form", error);
            }
        };
        fetchDependencies();
    }, [jobRoles, employmentRegimes]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setFieldErrors({});
        setFormError(null);
        try {
            const payload = {
                name,
                registrationNumber,
                jobRoleId: jobRoleId ? Number(jobRoleId) : null,
                employmentRegimeId: employmentRegimeId ? Number(employmentRegimeId) : null,
                hireDate: hireDate || null,
                phone,
                document,
                active
            };

            if (canViewFinance) {
                payload.payRate = payRate ? Number(payRate) : 0;
                payload.additionalPercentage = additionalPercentage ? Number(additionalPercentage) : 0;
            }

            if (employee) {
                await employees.update(employee.id, payload);
            } else {
                await employees.create(payload);
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
        <Dialog open={true} title={employee ? "Edit Employee" : "New Employee"} onClose={onClose}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                {formError && <div className="p-3 bg-red-100 text-red-700 rounded text-sm">{formError}</div>}
                <div className="grid grid-cols-2 gap-4">
                    <TextField label="Name" value={name} onChange={e => setName(e.target.value)} error={fieldErrors.name} />
                    <TextField label="Registration Number" value={registrationNumber} onChange={e => setRegistrationNumber(e.target.value)} />
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <div className="flex flex-col gap-1">
                        <label className="text-sm font-semibold">Job Role</label>
                        <select value={jobRoleId} onChange={e => setJobRoleId(e.target.value)} className="border border-line rounded px-3 py-2">
                            <option value="">Select a Role</option>
                            {rolesList.map(r => <option key={r.id} value={r.id}>{r.name}</option>)}
                        </select>
                    </div>
                    <div className="flex flex-col gap-1">
                        <label className="text-sm font-semibold">Employment Regime</label>
                        <select value={employmentRegimeId} onChange={e => setEmploymentRegimeId(e.target.value)} className="border border-line rounded px-3 py-2">
                            <option value="">Select a Regime</option>
                            {regimesList.map(r => <option key={r.id} value={r.id}>{r.label}</option>)}
                        </select>
                    </div>
                </div>

                {canViewFinance && (
                    <div className="grid grid-cols-2 gap-4">
                        <TextField label="Pay Rate" type="number" step="0.01" value={payRate} onChange={e => setPayRate(e.target.value)} />
                        <TextField label="Additional % (e.g. 30 for hazard)" type="number" step="0.01" value={additionalPercentage} onChange={e => setAdditionalPercentage(e.target.value)} />
                    </div>
                )}

                <div className="grid grid-cols-3 gap-4">
                    <TextField label="Hire Date" type="date" value={hireDate} onChange={e => setHireDate(e.target.value)} />
                    <TextField label="Phone" value={phone} onChange={e => setPhone(e.target.value)} />
                    <TextField label="Document (CPF/RG)" value={document} onChange={e => setDocument(e.target.value)} placeholder={employee?.documentMasked || ''} />
                </div>
                
                {employee && (
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