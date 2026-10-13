import { SelectField } from '../../ui/SelectField';
import { useState, useEffect } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Dialog } from '../../ui/Dialog';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { useT } from '../../preferences/preferencesContext'

export function JobRoleFormDialog({ jobRole, onClose, onSave }) {
    const t = useT()
    const { jobRoles, departments } = useContainer();
    const [name, setName] = useState(jobRole ? jobRole.name : '');
    const [departmentId, setDepartmentId] = useState(jobRole ? jobRole.departmentId : '');
    const [active, setActive] = useState(jobRole ? jobRole.active : true);

    const [departmentsList, setDepartmentsList] = useState([]);
    const [fieldErrors, setFieldErrors] = useState({});
    const [formError, setFormError] = useState(null);

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
        setFieldErrors({});
        setFormError(null);
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
            if (error.isValidation) {
                setFieldErrors(error.fieldErrors);
            } else {
                setFormError(error.message || t('jobRoleFormDialog.anUnexpectedErrorOccurred'));
            }
        }
    };

    return (
        <Dialog open={true} title={jobRole ? t('jobRoleFormDialog.editJobRole') : t('jobRoleFormDialog.newJobRole')} onClose={onClose}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4 p-6">
                {formError && <div className="p-3 bg-danger-wash text-danger rounded text-sm">{formError}</div>}
                <TextField
                    label={t('common.name')}
                    value={name}
                    onChange={e => setName(e.target.value)}
                    error={fieldErrors.name}
                />

                <SelectField
                    label={t('jobRoleFormDialog.department')}
                    value={departmentId}
                    onChange={e => setDepartmentId(e.target.value)}
                    error={fieldErrors.departmentId}
                    options={[
                        ...(jobRole ? [] : [{ value: '', label: t('jobRoleFormDialog.selectADepartment') }]),
                        ...departmentsList.map(dep => ({ value: dep.id, label: dep.name }))
                    ]}
                />

                {jobRole && (
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