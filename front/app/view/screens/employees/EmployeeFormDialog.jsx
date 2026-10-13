import { SelectField } from '../../ui/SelectField';
import { useState, useEffect } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Dialog } from '../../ui/Dialog';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { useT } from '../../preferences/preferencesContext'

export function EmployeeFormDialog({ employee, onClose, onSave, canViewFinance }) {
    const t = useT()
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

                const activeRoles = rolesRes.items.filter(r => r.active || (employee && r.id === employee.jobRoleId));
                const activeRegimes = regimesRes.items.filter(r => r.active || (employee && r.id === employee.employmentRegimeId));

                setRolesList(activeRoles);
                setRegimesList(activeRegimes);
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
                setFormError(error.message || t('employeeFormDialog.anUnexpectedErrorOccurred'));
            }
        }
    };

    return (
        <Dialog open={true} title={employee ? t('employeeFormDialog.editEmployee') : t('employeeFormDialog.newEmployee')} onClose={onClose}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4 p-6">
                {formError && <div className="p-3 bg-danger-wash text-danger rounded text-sm">{formError}</div>}
                <div className="grid grid-cols-2 gap-4">
                    <TextField label={t('common.name')} value={name} onChange={e => setName(e.target.value)} error={fieldErrors.name} />
                    <TextField
                        label={t('employeeFormDialog.registrationNumber')}
                        value={registrationNumber}
                        onChange={e => setRegistrationNumber(e.target.value)}
                        error={fieldErrors.registrationNumber}
                    />
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <SelectField
                        label={t('employeeFormDialog.jobRole')}
                        value={jobRoleId}
                        onChange={e => setJobRoleId(e.target.value)}
                        error={fieldErrors.jobRoleId}
                        options={[
                            { value: '', label: t('employeeFormDialog.selectARole') },
                            ...rolesList.map(r => ({ value: r.id, label: r.name }))
                        ]}
                    />

                    <SelectField
                        label={t('employeeFormDialog.employmentRegime')}
                        value={employmentRegimeId}
                        onChange={e => setEmploymentRegimeId(e.target.value)}
                        error={fieldErrors.employmentRegimeId}
                        options={[
                            { value: '', label: t('employeeFormDialog.selectARegime') },
                            ...regimesList.map(r => ({ value: r.id, label: r.label }))
                        ]}
                    />
                </div>

                {canViewFinance && (
                    <div className="grid grid-cols-2 gap-4">
                        <TextField label={t('employeeFormDialog.payRate')} type="number" step="0.01" value={payRate} onChange={e => setPayRate(e.target.value)} />
                        <TextField label={t('employeeFormDialog.additionalEG30For')} type="number" step="0.01" value={additionalPercentage} onChange={e => setAdditionalPercentage(e.target.value)} />
                    </div>
                )}

                <div className="grid grid-cols-3 gap-4">
                    <TextField label={t('employeeFormDialog.hireDate')} type="date" value={hireDate} onChange={e => setHireDate(e.target.value)} />
                    <TextField label={t('employeeFormDialog.phone')} value={phone} onChange={e => setPhone(e.target.value)} />
                    <TextField
                        label={t('employeeFormDialog.documentCpfRg')}
                        value={document}
                        onChange={e => setDocument(e.target.value)}
                        placeholder={employee?.documentMasked || ''}
                        error={fieldErrors.document}
                    />
                </div>

                {employee && (
                    <label className="flex items-center gap-2">
                        <input type="checkbox" checked={active} onChange={e => setActive(e.target.checked)} />
                        {t('common.active')}
                    </label>
                )}

                <div className="flex gap-2 mt-4 justify-end">
                    <Button type="button" variant="outline" onClick={onClose}>{t('common.cancel')}</Button>
                    <Button type="submit">{t('common.save')}</Button>
                </div>
            </form>
        </Dialog>
    );
}