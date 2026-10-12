import { useState, useEffect } from 'react';
import { useContainer } from '../../providers/containerContext';
import { useSession } from '../../providers/sessionContext';
import { Dialog } from '../../ui/Dialog';
import { Badge } from '../../ui/Badge';
import { Button } from '../../ui/Button';
import { formatDate } from '../../format/dates';

export function EmployeeProfile({ employeeId, onClose }) {
    const { employees } = useContainer();
    const { can } = useSession();
    const canViewFinance = can('view_finance');

    const [profile, setProfile] = useState(null);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        let isMounted = true;

        async function loadProfile() {
            try {
                const data = await employees.getById(employeeId);
                if (isMounted) {
                    setProfile(data);
                    setIsLoading(false);
                }
            } catch (err) {
                if (isMounted) {
                    console.error("Error loading employee profile", err);
                    setError("Failed to load employee details.");
                    setIsLoading(false);
                }
            }
        }

        loadProfile();

        return () => { isMounted = false; };
    }, [employeeId, employees]);

    if (isLoading) return <Dialog open={true} title="Employee Profile" onClose={onClose}><div className="p-4">Loading...</div></Dialog>;
    if (error) return <Dialog open={true} title="Employee Profile" onClose={onClose}><div className="p-4 text-red-600">{error}</div></Dialog>;
    if (!profile) return null;

    const renderTrainingStatusBadge = (status) => {
        switch (status) {
            case 'Valid': return <Badge variant="success">Valid</Badge>;
            case 'Expiring': return <Badge variant="warning">Expiring Soon</Badge>;
            case 'Expired': return <Badge variant="danger">Expired</Badge>;
            case 'NoExpiry': return <Badge variant="info">No Expiry</Badge>;
            default: return <Badge>{status}</Badge>;
        }
    };

    return (
        <Dialog open={true} title="Employee Profile" onClose={onClose}>
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="space-y-6">
                    <section>
                        <h2 className="text-lg font-semibold border-b pb-2 mb-3">Personal Details</h2>
                        <div className="space-y-2 text-sm">
                            <p><span className="font-medium text-gray-500 w-32 inline-block">Name:</span> {profile.name}</p>
                            <p><span className="font-medium text-gray-500 w-32 inline-block">Registration:</span> {profile.registrationNumber || '-'}</p>
                            <p><span className="font-medium text-gray-500 w-32 inline-block">Document:</span> {profile.documentMasked || '-'}</p>
                            <p><span className="font-medium text-gray-500 w-32 inline-block">Phone:</span> {profile.phone || '-'}</p>
                            <p><span className="font-medium text-gray-500 w-32 inline-block">Status:</span> {profile.active ? 'Active' : 'Inactive'}</p>
                        </div>
                    </section>

                    <section>
                        <h2 className="text-lg font-semibold border-b pb-2 mb-3">Employment</h2>
                        <div className="space-y-2 text-sm">
                            <p><span className="font-medium text-gray-500 w-32 inline-block">Job Role:</span> {profile.jobRoleName || '-'}</p>
                            <p><span className="font-medium text-gray-500 w-32 inline-block">Regime:</span> {profile.employmentRegimeLabel || '-'}</p>
                            <p><span className="font-medium text-gray-500 w-32 inline-block">Hire Date:</span> {profile.hireDate ? formatDate(profile.hireDate) : '-'}</p>

                            {canViewFinance && (
                                <>
                                    <p><span className="font-medium text-gray-500 w-32 inline-block">Pay Rate:</span> R$ {profile.payRate?.toFixed(2) || '0.00'}</p>
                                    <p><span className="font-medium text-gray-500 w-32 inline-block">Additional:</span> {profile.additionalPercentage?.toFixed(2) || '0.00'}%</p>
                                </>
                            )}
                        </div>
                    </section>
                </div>

                <div className="space-y-6">
                    <section>
                        <h2 className="text-lg font-semibold border-b pb-2 mb-3">Current Team & Projects</h2>
                        {profile.currentTeamName ? (
                            <div className="text-sm space-y-2">
                                <p><span className="font-medium text-gray-500">Team:</span> {profile.currentTeamName}</p>
                                <p><span className="font-medium text-gray-500">Allocated since:</span> {profile.teamAllocationDate ? formatDate(profile.teamAllocationDate) : '-'}</p>

                                <div className="mt-3">
                                    <span className="font-medium text-gray-500 block mb-1">Active Projects:</span>
                                    {profile.currentProjects?.length > 0 ? (
                                        <ul className="list-disc list-inside ml-2">
                                            {profile.currentProjects.map(p => (
                                                <li key={p.id}>{p.code} - {p.name}</li>
                                            ))}
                                        </ul>
                                    ) : (
                                        <p className="text-gray-400 italic">No active projects assigned to this team.</p>
                                    )}
                                </div>
                            </div>
                        ) : (
                            <p className="text-sm text-gray-400 italic">Not currently assigned to any team.</p>
                        )}
                    </section>

                    <section>
                        <h2 className="text-lg font-semibold border-b pb-2 mb-3">Active Trainings & Certifications</h2>
                        {profile.trainings?.length > 0 ? (
                            <div className="space-y-3">
                                {profile.trainings.map(t => (
                                    <div key={t.id} className="text-sm bg-gray-50 p-3 rounded border">
                                        <div className="flex justify-between items-start mb-1">
                                            <span className="font-medium">{t.typeName}</span>
                                            {renderTrainingStatusBadge(t.status)}
                                        </div>
                                        <p className="text-gray-500 text-xs">Issued: {formatDate(t.issueDate)} {t.expiryDate && `| Expires: ${formatDate(t.expiryDate)}`}</p>
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <p className="text-sm text-gray-400 italic">No valid training records found.</p>
                        )}
                    </section>
                </div>
            </div>

            <div className="p-4 border-t flex justify-end">
                <Button variant="outline" onClick={onClose}>Close</Button>
            </div>
        </Dialog>
    );
}