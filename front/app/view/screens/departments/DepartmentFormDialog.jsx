import { useState } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Dialog } from '../../ui/Dialog';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { useT } from '../../preferences/preferencesContext'

export function DepartmentFormDialog({ department, onClose, onSave }) {
    const t = useT()
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
                setFormError(error.message || t('departmentFormDialog.anUnexpectedErrorOccurred'));
            }
        }
    };

    return (
        <Dialog open={true} title={department ? t('departmentFormDialog.editDepartment') : t('departmentFormDialog.newDepartment')} onClose={onClose}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4 p-6">
                {formError && <div className="p-3 bg-danger-wash text-danger rounded text-sm">{formError}</div>}
                <TextField
                    label={t('common.name')}
                    value={name}
                    onChange={e => setName(e.target.value)}
                    error={fieldErrors.name}
                />

                {department && (
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